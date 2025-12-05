const { app, BrowserWindow, Menu, ipcMain, dialog } = require('electron');
const { autoUpdater } = require('electron-updater');
const path = require('path');
const sqlite3 = require('sqlite3').verbose();
const fs = require('fs');

// Add global error handlers
process.on('uncaughtException', (error) => {
  console.error('=== MAIN: Uncaught Exception:', error);
});

process.on('unhandledRejection', (reason, promise) => {
  console.error('=== MAIN: Unhandled Rejection at:', promise, 'reason:', reason);
});

console.log('=== MAIN: Starting Electron app ===');
console.log('=== MAIN: Node version:', process.version);
console.log('=== MAIN: Electron version:', process.versions.electron);
console.log('=== MAIN: Current working directory:', process.cwd());

// Keep a global reference of the window object
let mainWindow;
let db;

// Auto-updater configuration
autoUpdater.checkForUpdatesAndNotify = false; // We'll handle this manually
autoUpdater.autoDownload = false; // Ask user before downloading
autoUpdater.logger = console;

// Auto-updater event handlers
function setupAutoUpdater() {
  console.log('=== UPDATER: Setting up auto-updater ===');
  
  autoUpdater.on('checking-for-update', () => {
    console.log('=== UPDATER: Checking for update...');
  });

  autoUpdater.on('update-available', (info) => {
    console.log('=== UPDATER: Update available:', info.version);
    
    // Show dialog to user
    const response = dialog.showMessageBoxSync(mainWindow, {
      type: 'info',
      buttons: ['Download Update', 'Later'],
      title: 'Update Available',
      message: `A new version (${info.version}) is available!`,
      detail: 'Would you like to download and install the update? The application will restart after installation.',
      defaultId: 0,
      cancelId: 1
    });

    if (response === 0) {
      console.log('=== UPDATER: User chose to download update');
      autoUpdater.downloadUpdate();
    } else {
      console.log('=== UPDATER: User postponed update');
    }
  });

  autoUpdater.on('update-not-available', (info) => {
    console.log('=== UPDATER: Update not available. Current version:', info.version);
  });

  autoUpdater.on('error', (err) => {
    console.error('=== UPDATER: Error in auto-updater:', err);
  });

  autoUpdater.on('download-progress', (progressObj) => {
    let log_message = `=== UPDATER: Download progress: ${Math.round(progressObj.percent)}%`;
    log_message += ` (${Math.round(progressObj.transferred / 1024 / 1024)}MB / ${Math.round(progressObj.total / 1024 / 1024)}MB)`;
    console.log(log_message);
    
    // Send progress to renderer if window exists
    if (mainWindow && !mainWindow.isDestroyed()) {
      mainWindow.webContents.send('update-progress', progressObj);
    }
  });

  autoUpdater.on('update-downloaded', (info) => {
    console.log('=== UPDATER: Update downloaded, version:', info.version);
    
    const response = dialog.showMessageBoxSync(mainWindow, {
      type: 'info',
      buttons: ['Restart Now', 'Later'],
      title: 'Update Ready',
      message: 'Update has been downloaded successfully!',
      detail: 'The application will restart to apply the update. Any unsaved changes will be lost.',
      defaultId: 0,
      cancelId: 1
    });

    if (response === 0) {
      console.log('=== UPDATER: User chose to restart now');
      autoUpdater.quitAndInstall();
    } else {
      console.log('=== UPDATER: User postponed restart');
    }
  });
}

// Check for updates (with internet connection check)
async function checkForUpdates() {
  try {
    // Only check if we're not in development mode
    if (process.env.NODE_ENV === 'development' || !app.isPackaged) {
      console.log('=== UPDATER: Skipping update check in development mode');
      return;
    }

    console.log('=== UPDATER: Starting update check...');
    await autoUpdater.checkForUpdates();
  } catch (error) {
    console.error('=== UPDATER: Error checking for updates:', error.message);
    // Fail silently - don't bother user if update check fails
  }
}

function createWindow() {
  // Create the browser window
  const preloadPath = path.resolve(__dirname, 'preload.js');
  console.log('=== MAIN: Using absolute preload path:', preloadPath);
  console.log('=== MAIN: Creating window ===');
  console.log('=== MAIN: Current __dirname:', __dirname);
  console.log('=== MAIN: Preload script path:', preloadPath);
  console.log('=== MAIN: Preload script exists:', require('fs').existsSync(preloadPath));
  
  if (require('fs').existsSync(preloadPath)) {
    try {
      const stats = require('fs').statSync(preloadPath);
      console.log('=== MAIN: Preload file size:', stats.size, 'bytes');
      console.log('=== MAIN: Preload file permissions:', stats.mode.toString(8));
      
      // Try to read the first few lines
      const content = require('fs').readFileSync(preloadPath, 'utf8');
      console.log('=== MAIN: Preload file first 200 chars:', content.substring(0, 200));
    } catch (error) {
      console.error('=== MAIN: Error reading preload file:', error);
    }
  }
  
  console.log('=== MAIN: Creating BrowserWindow with webPreferences ===');
  
  mainWindow = new BrowserWindow({
    width: 1400,
    height: 900,
    minWidth: 1200,
    minHeight: 700,
    webPreferences: {
      nodeIntegration: false,
      contextIsolation: true,
      preload: preloadPath,
      enableRemoteModule: false
    },
    show: false
  });
  
  console.log('=== MAIN: BrowserWindow created successfully ===');

  // Load the app
  const indexPath = path.join(__dirname, 'index.html');
  console.log('Loading index.html from:', indexPath);
  console.log('Index.html exists:', require('fs').existsSync(indexPath));
  
  mainWindow.loadFile(indexPath);

  // Show window when ready to prevent visual flash
  mainWindow.once('ready-to-show', () => {
    console.log('Window ready to show');
    mainWindow.show();
  });

  // Handle any loading errors
  mainWindow.webContents.on('did-fail-load', (event, errorCode, errorDescription) => {
    console.error('Failed to load:', errorCode, errorDescription);
  });

  // Log when DOM is ready
  mainWindow.webContents.on('dom-ready', () => {
    console.log('DOM ready in renderer process');
  });

  // Handle preload script errors
  mainWindow.webContents.on('preload-error', (event, preloadPath, error) => {
    console.error('Preload script error:', preloadPath, error);
  });

  // More detailed debugging
  mainWindow.webContents.on('did-finish-load', () => {
    console.log('=== MAIN: Finished loading page ===');
    // Check if preload script was actually loaded
    mainWindow.webContents.executeJavaScript(`
      console.log('=== MAIN->RENDERER: Checking APIs');
      console.log('=== MAIN->RENDERER: window.simpleTest:', !!window.simpleTest);
      if (window.simpleTest) {
        console.log('=== MAIN->RENDERER: simpleTest.hello:', window.simpleTest.hello);
        console.log('=== MAIN->RENDERER: simpleTest.getVersion():', window.simpleTest.getVersion());
      }
      console.log('=== MAIN->RENDERER: window.electronAPI:', !!window.electronAPI);
    `)
      .catch(err => console.error('=== MAIN: Failed to execute JavaScript:', err));
  });
  
  // Add more event listeners for debugging
  mainWindow.webContents.on('crashed', () => {
    console.error('=== MAIN: Renderer process crashed ===');
  });
  
  mainWindow.webContents.on('unresponsive', () => {
    console.error('=== MAIN: Renderer process unresponsive ===');
  });
  
  mainWindow.webContents.on('responsive', () => {
    console.log('=== MAIN: Renderer process responsive again ===');
  });

  // Open DevTools in development
  if (process.argv.includes('--dev')) {
    mainWindow.webContents.openDevTools();
  }

  // Emitted when the window is closed
  mainWindow.on('closed', () => {
    mainWindow = null;
  });

  // Set up menu
  createMenu();
}

function createMenu() {
  const template = [
    {
      label: 'File',
      submenu: [
        {
          label: 'New League',
          accelerator: 'CmdOrCtrl+Shift+L',
          click: () => {
            mainWindow.webContents.send('menu-action', 'new-league');
          }
        },
        {
          label: 'New Team',
          accelerator: 'CmdOrCtrl+Shift+T',
          click: () => {
            mainWindow.webContents.send('menu-action', 'new-team');
          }
        },
        { type: 'separator' },
        {
          label: 'Exit',
          accelerator: process.platform === 'darwin' ? 'Cmd+Q' : 'Ctrl+Q',
          click: () => {
            app.quit();
          }
        }
      ]
    },
    {
      label: 'Game',
      submenu: [
        {
          label: 'New Game',
          accelerator: 'CmdOrCtrl+N',
          click: () => {
            mainWindow.webContents.send('menu-action', 'new-game');
          }
        },
        {
          label: 'Team Foul Info',
          accelerator: 'CmdOrCtrl+R',
          click: () => {
            mainWindow.webContents.send('menu-action', 'reset-fouls');
          }
        },
        {
          label: 'Swap Teams',
          accelerator: 'CmdOrCtrl+S',
          click: () => {
            mainWindow.webContents.send('menu-action', 'swap-teams');
          }
        }
      ]
    },
    {
      label: 'View',
      submenu: [
        {
          label: 'Reload',
          accelerator: 'CmdOrCtrl+R',
          click: () => {
            mainWindow.reload();
          }
        },
        {
          label: 'Force Reload',
          accelerator: 'CmdOrCtrl+Shift+R',
          click: () => {
            mainWindow.webContents.reloadIgnoringCache();
          }
        },
        {
          label: 'Toggle Developer Tools',
          accelerator: process.platform === 'darwin' ? 'Alt+Cmd+I' : 'Ctrl+Shift+I',
          click: () => {
            mainWindow.webContents.toggleDevTools();
          }
        }
      ]
    },
    {
      label: 'Help',
      submenu: [
        {
          label: 'Check for Updates',
          click: async () => {
            try {
              console.log('=== MENU: Check for updates clicked');
              
              // Show checking dialog
              const checkingDialog = dialog.showMessageBox(mainWindow, {
                type: 'info',
                buttons: ['Cancel'],
                title: 'Checking for Updates',
                message: 'Checking for updates...',
                detail: 'Please wait while we check for the latest version.'
              });

              await checkForUpdates();
              
              // Close the checking dialog if it's still open
              if (checkingDialog) {
                // The dialog might have been closed by the update process
              }
            } catch (error) {
              console.error('=== MENU: Error checking for updates:', error);
              dialog.showMessageBox(mainWindow, {
                type: 'error',
                buttons: ['OK'],
                title: 'Update Check Failed',
                message: 'Failed to check for updates',
                detail: 'Please check your internet connection and try again later.'
              });
            }
          }
        },
        { type: 'separator' },
        {
          label: 'About Basketball Roster Manager',
          click: () => {
            dialog.showMessageBox(mainWindow, {
              type: 'info',
              buttons: ['OK'],
              title: 'About',
              message: 'Basketball Roster Manager',
              detail: `Version: ${app.getVersion()}\nA cross-platform roster management tool for basketball games.`
            });
          }
        }
      ]
    }
  ];

  // macOS specific menu adjustments
  if (process.platform === 'darwin') {
    template.unshift({
      label: app.getName(),
      submenu: [
        { label: 'About ' + app.getName(), role: 'about' },
        { type: 'separator' },
        { label: 'Services', role: 'services', submenu: [] },
        { type: 'separator' },
        { label: 'Hide ' + app.getName(), accelerator: 'Command+H', role: 'hide' },
        { label: 'Hide Others', accelerator: 'Command+Shift+H', role: 'hideothers' },
        { label: 'Show All', role: 'unhide' },
        { type: 'separator' },
        { label: 'Quit', accelerator: 'Command+Q', click: () => app.quit() }
      ]
    });
  }

  const menu = Menu.buildFromTemplate(template);
  Menu.setApplicationMenu(menu);
}

function initializeDatabase() {
  try {
    const userDataPath = app.getPath('userData');
    const dbPath = path.join(userDataPath, 'basketball-roster.db');
    
    console.log('Initializing database...');
    console.log('User data path:', userDataPath);
    console.log('Database path:', dbPath);
    
    // Ensure the user data directory exists
    const fs = require('fs');
    if (!fs.existsSync(userDataPath)) {
      console.log('Creating user data directory:', userDataPath);
      fs.mkdirSync(userDataPath, { recursive: true });
    }
    
    // Create database connection
    db = new sqlite3.Database(dbPath, (err) => {
      if (err) {
        console.error('Database connection error:', err);
        return;
      }
      console.log('Connected to SQLite database');
      
      // Run migrations after successful connection
      runMigrations();
    });
  
  // Create tables if they don't exist
  const createTables = [
    `CREATE TABLE IF NOT EXISTS leagues (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      name TEXT NOT NULL,
      foul_reset_period TEXT NOT NULL DEFAULT 'half',
      bonus_fouls INTEGER NOT NULL DEFAULT 7,
      double_bonus_fouls INTEGER NOT NULL DEFAULT 10,
      created_at DATETIME DEFAULT CURRENT_TIMESTAMP
    )`,
    
    `CREATE TABLE IF NOT EXISTS teams (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      league_id INTEGER NOT NULL,
      name TEXT NOT NULL,
      color TEXT DEFAULT '#000000',
      created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      FOREIGN KEY (league_id) REFERENCES leagues (id)
    )`,
    
    `CREATE TABLE IF NOT EXISTS players (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      team_id INTEGER NOT NULL,
      jersey_number TEXT NOT NULL,
      name TEXT NOT NULL,
      description TEXT,
      is_active INTEGER DEFAULT 1,
      created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      FOREIGN KEY (team_id) REFERENCES teams (id)
    )`,
    
    `CREATE TABLE IF NOT EXISTS game_sessions (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      home_team_id INTEGER,
      away_team_id INTEGER,
      current_period INTEGER DEFAULT 1,
      max_periods INTEGER DEFAULT 2,
      created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      FOREIGN KEY (home_team_id) REFERENCES teams (id),
      FOREIGN KEY (away_team_id) REFERENCES teams (id)
    )`,
    
    `CREATE TABLE IF NOT EXISTS game_stats (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      session_id INTEGER NOT NULL,
      player_id INTEGER NOT NULL,
      fouls_period1 INTEGER DEFAULT 0,
      fouls_period2 INTEGER DEFAULT 0,
      fouls_period3 INTEGER DEFAULT 0,
      fouls_period4 INTEGER DEFAULT 0,
      field_goals INTEGER DEFAULT 0,
      three_pointers INTEGER DEFAULT 0,
      free_throws INTEGER DEFAULT 0,
      is_checked_in INTEGER DEFAULT 0,
      updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      FOREIGN KEY (session_id) REFERENCES game_sessions (id),
      FOREIGN KEY (player_id) REFERENCES players (id)
    )`
  ];
  
  // Run table creation serially
  db.serialize(() => {
    createTables.forEach(sql => {
      db.run(sql);
    });
    
    // Insert default leagues if none exist
    db.get('SELECT COUNT(*) as count FROM leagues', (err, row) => {
      if (!err && row.count === 0) {
        const defaultLeagues = [
          ['NCAA - Men', 'half', 7, 10],
          ['NCAA - Women', 'half', 7, 10],
          ['High School - Boys', 'quarter', 7, 10],
          ['High School - Girls', 'quarter', 5, 5],
          ['Varsity - Boys', 'half', 7, 10],
          ['Varsity - Girls', 'half', 7, 10]
        ];
        
        const stmt = db.prepare('INSERT INTO leagues (name, foul_reset_period, bonus_fouls, double_bonus_fouls) VALUES (?, ?, ?, ?)');
        defaultLeagues.forEach(league => {
          stmt.run(league);
        });
        stmt.finalize();
        console.log('Default leagues inserted');
      } else if (err) {
        console.error('Error checking leagues:', err);
      } else {
        console.log('Leagues already exist, count:', row.count);
      }
    });
  });
  
  console.log('Database initialization completed');
  
  } catch (error) {
    console.error('Database initialization failed:', error);
    throw error;
  }
}

// Database migration function
function runMigrations() {
  console.log('Running database migrations...');
  
  // Migration: Change graduating_class column to description
  db.serialize(() => {
    // Check if the old column exists
    db.all("PRAGMA table_info(players)", (err, columns) => {
      if (err) {
        console.error('Error checking table info:', err);
        return;
      }
      
      const hasGraduatingClass = columns.some(col => col.name === 'graduating_class');
      const hasDescription = columns.some(col => col.name === 'description');
      
      if (hasGraduatingClass && !hasDescription) {
        console.log('Migrating graduating_class to description...');
        
        // Add the new description column
        db.run("ALTER TABLE players ADD COLUMN description TEXT", (err) => {
          if (err) {
            console.error('Error adding description column:', err);
            return;
          }
          
          // Copy data from graduating_class to description
          db.run("UPDATE players SET description = graduating_class WHERE graduating_class IS NOT NULL", (err) => {
            if (err) {
              console.error('Error copying data to description column:', err);
              return;
            }
            
            // Note: SQLite doesn't support dropping columns directly, 
            // but we can leave the old column for compatibility
            console.log('Migration completed: graduating_class -> description');
          });
        });
      } else if (!hasDescription) {
        // If neither column exists, add description column
        db.run("ALTER TABLE players ADD COLUMN description TEXT", (err) => {
          if (err) {
            console.error('Error adding description column:', err);
          } else {
            console.log('Added description column to players table');
          }
        });
      } else {
        console.log('Database schema is up to date');
      }
    });
  });
}

// Database API methods (using Promises for async operations)
const dbAPI = {
  getLeagues: () => {
    return new Promise((resolve, reject) => {
      db.all('SELECT * FROM leagues ORDER BY name', (err, rows) => {
        if (err) reject(err);
        else resolve(rows || []);
      });
    });
  },
  
  getTeams: (leagueId) => {
    return new Promise((resolve, reject) => {
      db.all('SELECT * FROM teams WHERE league_id = ? ORDER BY name', [leagueId], (err, rows) => {
        if (err) reject(err);
        else resolve(rows || []);
      });
    });
  },
  
  getPlayers: (teamId) => {
    return new Promise((resolve, reject) => {
      db.all(`
        SELECT * FROM players 
        WHERE team_id = ? AND is_active = 1
      `, [teamId], (err, rows) => {
        if (err) reject(err);
        else resolve(rows || []);
      });
    });
  },
  
  createLeague: (name, foulResetPeriod, bonusFouls, doubleBonusFouls) => {
    return new Promise((resolve, reject) => {
      db.run(`
        INSERT INTO leagues (name, foul_reset_period, bonus_fouls, double_bonus_fouls) 
        VALUES (?, ?, ?, ?)
      `, [name, foulResetPeriod, bonusFouls, doubleBonusFouls], function(err) {
        if (err) reject(err);
        else resolve({ lastID: this.lastID, changes: this.changes });
      });
    });
  },
  
  createTeam: (leagueId, name, color) => {
    return new Promise((resolve, reject) => {
      db.run('INSERT INTO teams (league_id, name, color) VALUES (?, ?, ?)', 
        [leagueId, name, color], function(err) {
        if (err) reject(err);
        else resolve({ lastID: this.lastID, changes: this.changes });
      });
    });
  },
  
  createPlayer: (teamId, jerseyNumber, name, graduatingClass) => {
    return new Promise((resolve, reject) => {
      db.run(`
        INSERT INTO players (team_id, jersey_number, name, description) 
        VALUES (?, ?, ?, ?)
      `, [teamId, jerseyNumber, name, graduatingClass || ''], function(err) {
        if (err) reject(err);
        else resolve({ lastID: this.lastID, changes: this.changes });
      });
    });
  },
  
  updatePlayer: (playerId, jerseyNumber, name, graduatingClass) => {
    return new Promise((resolve, reject) => {
      db.run(`
        UPDATE players 
        SET jersey_number = ?, name = ?, description = ? 
        WHERE id = ?
      `, [jerseyNumber, name, graduatingClass || '', playerId], function(err) {
        if (err) reject(err);
        else resolve({ lastID: this.lastID, changes: this.changes });
      });
    });
  },

  updateLeague: (leagueId, name, foulResetPeriod, bonusFouls, doubleBonusFouls) => {
    return new Promise((resolve, reject) => {
      db.run(`
        UPDATE leagues 
        SET name = ?, foul_reset_period = ?, bonus_fouls = ?, double_bonus_fouls = ? 
        WHERE id = ?
      `, [name, foulResetPeriod, bonusFouls, doubleBonusFouls, leagueId], function(err) {
        if (err) reject(err);
        else resolve({ lastID: this.lastID, changes: this.changes });
      });
    });
  },

  updateTeam: (teamId, name, color) => {
    return new Promise((resolve, reject) => {
      db.run(`
        UPDATE teams 
        SET name = ?, color = ? 
        WHERE id = ?
      `, [name, color, teamId], function(err) {
        if (err) reject(err);
        else resolve({ lastID: this.lastID, changes: this.changes });
      });
    });
  },
  
  deletePlayer: (playerId) => {
    return new Promise((resolve, reject) => {
      db.run('UPDATE players SET is_active = 0 WHERE id = ?', [playerId], function(err) {
        if (err) reject(err);
        else resolve({ lastID: this.lastID, changes: this.changes });
      });
    });
  },
  
  deleteLeague: (leagueId) => {
    return new Promise((resolve, reject) => {
      // First check if league has any teams
      db.get('SELECT COUNT(*) as count FROM teams WHERE league_id = ?', [leagueId], (err, row) => {
        if (err) reject(err);
        else if (row.count > 0) {
          reject(new Error(`Cannot delete league: ${row.count} teams are associated with this league`));
        } else {
          // Safe to delete league
          db.run('DELETE FROM leagues WHERE id = ?', [leagueId], function(err) {
            if (err) reject(err);
            else resolve({ changes: this.changes });
          });
        }
      });
    });
  },
  
  deleteTeam: (teamId) => {
    return new Promise((resolve, reject) => {
      // First get count of players on this team
      db.get('SELECT COUNT(*) as count FROM players WHERE team_id = ? AND is_active = 1', [teamId], (err, row) => {
        if (err) reject(err);
        else {
          const playerCount = row.count;
          // Delete all players first (set inactive)
          db.run('UPDATE players SET is_active = 0 WHERE team_id = ?', [teamId], function(err) {
            if (err) reject(err);
            else {
              // Then delete the team
              db.run('DELETE FROM teams WHERE id = ?', [teamId], function(err) {
                if (err) reject(err);
                else resolve({ changes: this.changes, playersDeleted: playerCount });
              });
            }
          });
        }
      });
    });
  }
};

// IPC handlers
function setupIPC() {
  // Database queries
  ipcMain.handle('db:getLeagues', async () => {
    try {
      return await dbAPI.getLeagues();
    } catch (error) {
      console.error('Error getting leagues:', error);
      throw error;
    }
  });
  
  ipcMain.handle('db:getTeams', async (_, leagueId) => {
    try {
      return await dbAPI.getTeams(leagueId);
    } catch (error) {
      console.error('Error getting teams:', error);
      throw error;
    }
  });
  
  ipcMain.handle('db:getPlayers', async (_, teamId) => {
    try {
      console.log('Getting players for team ID:', teamId);
      const players = await dbAPI.getPlayers(teamId);
      console.log('Found players:', players.length);
      return players;
    } catch (error) {
      console.error('Error getting players for team', teamId, ':', error);
      console.error('Error details:', error.message);
      console.error('Error stack:', error.stack);
      throw error;
    }
  });
  
  // Create operations
  ipcMain.handle('db:createLeague', async (_, name, foulResetPeriod, bonusFouls, doubleBonusFouls) => {
    try {
      return await dbAPI.createLeague(name, foulResetPeriod, bonusFouls, doubleBonusFouls);
    } catch (error) {
      console.error('Error creating league:', error);
      throw error;
    }
  });
  
  ipcMain.handle('db:createTeam', async (_, leagueId, name, color) => {
    try {
      return await dbAPI.createTeam(leagueId, name, color);
    } catch (error) {
      console.error('Error creating team:', error);
      throw error;
    }
  });
  
  ipcMain.handle('db:createPlayer', async (_, teamId, jerseyNumber, name, graduatingClass) => {
    try {
      return await dbAPI.createPlayer(teamId, jerseyNumber, name, graduatingClass);
    } catch (error) {
      console.error('Error creating player:', error);
      throw error;
    }
  });
  
  // Update operations
  ipcMain.handle('db:updatePlayer', async (_, playerId, jerseyNumber, name, graduatingClass) => {
    try {
      return await dbAPI.updatePlayer(playerId, jerseyNumber, name, graduatingClass);
    } catch (error) {
      console.error('Error updating player:', error);
      throw error;
    }
  });

  ipcMain.handle('db:updateLeague', async (_, leagueId, name, foulResetPeriod, bonusFouls, doubleBonusFouls) => {
    try {
      return await dbAPI.updateLeague(leagueId, name, foulResetPeriod, bonusFouls, doubleBonusFouls);
    } catch (error) {
      console.error('Error updating league:', error);
      throw error;
    }
  });

  ipcMain.handle('db:updateTeam', async (_, teamId, name, color) => {
    try {
      return await dbAPI.updateTeam(teamId, name, color);
    } catch (error) {
      console.error('Error updating team:', error);
      throw error;
    }
  });
  
  // Delete operations
  ipcMain.handle('db:deletePlayer', async (_, playerId) => {
    try {
      return await dbAPI.deletePlayer(playerId);
    } catch (error) {
      console.error('Error deleting player:', error);
      throw error;
    }
  });
  
  ipcMain.handle('db:deleteLeague', async (_, leagueId) => {
    try {
      return await dbAPI.deleteLeague(leagueId);
    } catch (error) {
      console.error('Error deleting league:', error);
      throw error;
    }
  });
  
  ipcMain.handle('db:deleteTeam', async (_, teamId) => {
    try {
      return await dbAPI.deleteTeam(teamId);
    } catch (error) {
      console.error('Error deleting team:', error);
      throw error;
    }
  });

  // Auto-updater IPC handlers
  ipcMain.handle('updater:checkForUpdates', async () => {
    try {
      console.log('=== UPDATER: Manual update check requested');
      const result = await autoUpdater.checkForUpdates();
      return result;
    } catch (error) {
      console.error('=== UPDATER: Error in manual update check:', error);
      throw error;
    }
  });

  ipcMain.handle('updater:downloadUpdate', () => {
    try {
      console.log('=== UPDATER: Manual download requested');
      autoUpdater.downloadUpdate();
    } catch (error) {
      console.error('=== UPDATER: Error downloading update:', error);
      throw error;
    }
  });

  ipcMain.handle('updater:quitAndInstall', () => {
    try {
      console.log('=== UPDATER: Quit and install requested');
      autoUpdater.quitAndInstall();
    } catch (error) {
      console.error('=== UPDATER: Error quitting and installing:', error);
      throw error;
    }
  });
}

// App event handlers
app.whenReady().then(() => {
  console.log('App ready, initializing...');
  
  try {
    initializeDatabase();
    setupIPC();
    setupAutoUpdater();
    createWindow();
    console.log('App initialization completed');
    
    // Check for updates after a short delay to let the app fully load
    setTimeout(() => {
      checkForUpdates();
    }, 3000);
  } catch (error) {
    console.error('App initialization failed:', error);
  }

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) {
      createWindow();
    }
  });
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});

app.on('will-quit', () => {
  if (db) {
    db.close();
  }
});