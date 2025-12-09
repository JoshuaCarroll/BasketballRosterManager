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

console.log('=== UPDATER INIT: Configuring auto-updater...');
console.log('=== UPDATER INIT: electron-updater version:', require('electron-updater/package.json').version);

// Auto-updater event handlers
function setupAutoUpdater() {
  console.log('=== UPDATER: Setting up auto-updater ===');
  console.log('=== UPDATER: App version:', app.getVersion());
  console.log('=== UPDATER: App packaged:', app.isPackaged);
  console.log('=== UPDATER: Platform:', process.platform);
  console.log('=== UPDATER: Arch:', process.arch);
  
  // Configure updater with more verbose logging
  autoUpdater.logger = {
    info: (message) => console.log('=== UPDATER INFO:', message),
    warn: (message) => console.warn('=== UPDATER WARN:', message),
    error: (message) => console.error('=== UPDATER ERROR:', message),
    debug: (message) => console.log('=== UPDATER DEBUG:', message)
  };
  
  autoUpdater.on('checking-for-update', () => {
    console.log('=== UPDATER: Event - checking-for-update triggered');
    console.log('=== UPDATER: Feed URL:', autoUpdater.getFeedURL());
  });

  autoUpdater.on('update-available', (info) => {
    console.log('=== UPDATER: Event - update-available triggered');
    console.log('=== UPDATER: Update available:', JSON.stringify(info, null, 2));
    
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
    console.log('=== UPDATER: Event - update-not-available triggered');
    console.log('=== UPDATER: Update not available. Info:', JSON.stringify(info, null, 2));
    console.log('=== UPDATER: Current version:', app.getVersion());
  });

  autoUpdater.on('error', (err) => {
    console.error('=== UPDATER: Event - error triggered');
    console.error('=== UPDATER: Error details:', {
      message: err.message,
      stack: err.stack,
      code: err.code,
      name: err.name
    });
    console.error('=== UPDATER: Full error object:', err);
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
    console.log('=== UPDATER: checkForUpdates() called');
    console.log('=== UPDATER: NODE_ENV:', process.env.NODE_ENV);
    console.log('=== UPDATER: app.isPackaged:', app.isPackaged);
    
    // Only check if we're not in development mode
    if (process.env.NODE_ENV === 'development' || !app.isPackaged) {
      console.log('=== UPDATER: Skipping update check in development mode');
      return 'dev-mode-skip';
    }

    console.log('=== UPDATER: Starting update check...');
    console.log('=== UPDATER: Current app version:', app.getVersion());
    console.log('=== UPDATER: Update feed URL:', autoUpdater.getFeedURL());
    
    // Test network connectivity first
    console.log('=== UPDATER: Testing network connectivity...');
    try {
      const https = require('https');
      await new Promise((resolve, reject) => {
        const req = https.request('https://api.github.com/repos/JoshuaCarroll/BasketballRosterManager/releases/latest', {
          method: 'HEAD',
          timeout: 10000
        }, (res) => {
          console.log('=== UPDATER: GitHub API response status:', res.statusCode);
          resolve(res);
        });
        req.on('error', (err) => {
          console.error('=== UPDATER: Network test failed:', err.message);
          reject(err);
        });
        req.on('timeout', () => {
          console.error('=== UPDATER: Network test timed out');
          req.destroy();
          reject(new Error('Network test timeout'));
        });
        req.end();
      });
      console.log('=== UPDATER: Network connectivity test passed');
    } catch (netError) {
      console.error('=== UPDATER: Network connectivity test failed:', netError.message);
      throw new Error('Network connectivity test failed: ' + netError.message);
    }
    
    // Set a timeout for the update check
    const updatePromise = autoUpdater.checkForUpdates();
    const timeoutPromise = new Promise((_, reject) => {
      setTimeout(() => reject(new Error('Update check timeout after 30 seconds')), 30000);
    });
    
    console.log('=== UPDATER: Calling autoUpdater.checkForUpdates()...');
    const result = await Promise.race([updatePromise, timeoutPromise]);
    console.log('=== UPDATER: checkForUpdates completed successfully:', result);
    return result;
  } catch (error) {
    console.error('=== UPDATER: Error in checkForUpdates:');
    console.error('=== UPDATER: Error message:', error.message);
    console.error('=== UPDATER: Error stack:', error.stack);
    console.error('=== UPDATER: Error code:', error.code);
    console.error('=== UPDATER: Full error:', error);
    throw error; // Re-throw for caller to handle
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

let pointColumnsMenuItem;

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
          label: 'Show Point Columns',
          type: 'checkbox',
          checked: true,
          click: (menuItem) => {
            mainWindow.webContents.send('menu-action', 'toggle-point-columns', menuItem.checked);
          },
          id: 'point-columns-menu'
        }
      ]
    },
    {
      label: 'Help',
      submenu: [
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
        },
        {
          label: 'Check for Updates',
          click: async () => {
            try {
              console.log('=== MENU: Check for updates clicked');
              console.log('=== MENU: App version:', app.getVersion());
              console.log('=== MENU: App packaged:', app.isPackaged);
              
              const result = await checkForUpdates();
              console.log('=== MENU: Update check result:', result);
              
              if (result === 'dev-mode-skip') {
                dialog.showMessageBox(mainWindow, {
                  type: 'info',
                  buttons: ['OK'],
                  title: 'Development Mode',
                  message: 'Update checking is disabled in development mode',
                  detail: 'Updates are only checked in packaged/production builds.'
                });
                return;
              }
              
              // If we get here and no update dialogs were shown, it means no update was available
              console.log('=== MENU: No update available or check completed silently');
              
            } catch (error) {
              console.error('=== MENU: Error checking for updates:');
              console.error('=== MENU: Error message:', error.message);
              console.error('=== MENU: Error stack:', error.stack);
              console.error('=== MENU: Full error:', error);
              
              let errorDetail = `Error: ${error.message}`;
              if (error.message.includes('timeout')) {
                errorDetail = 'The update check timed out. Please check your internet connection and try again.';
              } else if (error.message.includes('ENOTFOUND') || error.message.includes('network')) {
                errorDetail = 'Network error. Please check your internet connection and try again.';
              } else if (error.code) {
                errorDetail = `Error code: ${error.code}. ${error.message}`;
              }
              
              dialog.showMessageBox(mainWindow, {
                type: 'error',
                buttons: ['OK'],
                title: 'Update Check Failed',
                message: 'Failed to check for updates',
                detail: errorDetail
              });
            }
          }
        },
        { type: 'separator' },
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
        { type: 'separator' },
        {
          label: 'Toggle Developer Tools',
          accelerator: process.platform === 'darwin' ? 'Alt+Cmd+I' : 'Ctrl+Shift+I',
          click: () => {
            mainWindow.webContents.toggleDevTools();
          }
        },
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
      max_player_fouls INTEGER NOT NULL DEFAULT 5,
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
  
  // Migration: Add max_player_fouls column to leagues table
  db.serialize(() => {
    db.all("PRAGMA table_info(leagues)", (err, columns) => {
      if (err) {
        console.error('Error checking leagues table info:', err);
        return;
      }
      
      const hasMaxPlayerFouls = columns.some(col => col.name === 'max_player_fouls');
      
      if (!hasMaxPlayerFouls) {
        console.log('Adding max_player_fouls column to leagues table...');
        db.run("ALTER TABLE leagues ADD COLUMN max_player_fouls INTEGER NOT NULL DEFAULT 5", (err) => {
          if (err) {
            console.error('Error adding max_player_fouls column:', err);
          } else {
            console.log('Successfully added max_player_fouls column');
          }
        });
      } else {
        console.log('max_player_fouls column already exists');
      }
    });
  });
  
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

  // Migration: Create settings table if it doesn't exist
  db.serialize(() => {
    db.get("SELECT name FROM sqlite_master WHERE type='table' AND name='settings'", (err, table) => {
      if (err) {
        console.error('Error checking for settings table:', err);
        return;
      }
      
      if (!table) {
        console.log('Creating settings table...');
        db.run(`
          CREATE TABLE settings (
            key TEXT PRIMARY KEY,
            value TEXT,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
          )
        `, (err) => {
          if (err) {
            console.error('Error creating settings table:', err);
          } else {
            console.log('Successfully created settings table');
          }
        });
      } else {
        console.log('Settings table already exists');
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
  
  createLeague: (name, foulResetPeriod, bonusFouls, doubleBonusFouls, maxPlayerFouls = 5) => {
    return new Promise((resolve, reject) => {
      db.run(`
        INSERT INTO leagues (name, foul_reset_period, bonus_fouls, double_bonus_fouls, max_player_fouls) 
        VALUES (?, ?, ?, ?, ?)
      `, [name, foulResetPeriod, bonusFouls, doubleBonusFouls, maxPlayerFouls], function(err) {
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

  updateLeague: (leagueId, name, foulResetPeriod, bonusFouls, doubleBonusFouls, maxPlayerFouls = 5) => {
    return new Promise((resolve, reject) => {
      db.run(`
        UPDATE leagues 
        SET name = ?, foul_reset_period = ?, bonus_fouls = ?, double_bonus_fouls = ?, max_player_fouls = ? 
        WHERE id = ?
      `, [name, foulResetPeriod, bonusFouls, doubleBonusFouls, maxPlayerFouls, leagueId], function(err) {
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

  deleteAllPlayersForTeam: (teamId) => {
    return new Promise((resolve, reject) => {
      db.run('UPDATE players SET is_active = 0 WHERE team_id = ?', [teamId], function(err) {
        if (err) reject(err);
        else resolve({ changes: this.changes });
      });
    });
  },

  getSetting: (key) => {
    return new Promise((resolve, reject) => {
      db.get('SELECT value FROM settings WHERE key = ?', [key], (err, row) => {
        if (err) reject(err);
        else resolve(row ? row.value : null);
      });
    });
  },

  setSetting: (key, value) => {
    return new Promise((resolve, reject) => {
      db.run(`
        INSERT OR REPLACE INTO settings (key, value) 
        VALUES (?, ?)
      `, [key, value], function(err) {
        if (err) reject(err);
        else resolve({ changes: this.changes });
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
  ipcMain.handle('db:createLeague', async (_, name, foulResetPeriod, bonusFouls, doubleBonusFouls, maxPlayerFouls) => {
    try {
      return await dbAPI.createLeague(name, foulResetPeriod, bonusFouls, doubleBonusFouls, maxPlayerFouls);
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

  ipcMain.handle('db:updateLeague', async (_, leagueId, name, foulResetPeriod, bonusFouls, doubleBonusFouls, maxPlayerFouls) => {
    try {
      return await dbAPI.updateLeague(leagueId, name, foulResetPeriod, bonusFouls, doubleBonusFouls, maxPlayerFouls);
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

  ipcMain.handle('db:deleteAllPlayersForTeam', async (_, teamId) => {
    try {
      return await dbAPI.deleteAllPlayersForTeam(teamId);
    } catch (error) {
      console.error('Error deleting all players for team:', error);
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

  // Settings operations
  ipcMain.handle('settings:get', async (_, key) => {
    try {
      return await dbAPI.getSetting(key);
    } catch (error) {
      console.error('Error getting setting:', error);
      throw error;
    }
  });

  ipcMain.handle('settings:set', async (_, key, value) => {
    try {
      return await dbAPI.setSetting(key, value);
    } catch (error) {
      console.error('Error setting setting:', error);
      throw error;
    }
  });

  // Menu state management
  ipcMain.handle('menu:updatePointColumns', async (_, checked) => {
    try {
      const menu = Menu.getApplicationMenu();
      const menuItem = menu.getMenuItemById('point-columns-menu');
      if (menuItem) {
        menuItem.checked = checked;
      }
    } catch (error) {
      console.error('Error updating menu state:', error);
    }
  });

  // Auto-updater IPC handlers
  ipcMain.handle('updater:checkForUpdates', async () => {
    try {
      console.log('=== IPC UPDATER: Manual update check requested via IPC');
      console.log('=== IPC UPDATER: App version:', app.getVersion());
      console.log('=== IPC UPDATER: App packaged:', app.isPackaged);
      console.log('=== IPC UPDATER: Platform:', process.platform);
      
      const result = await checkForUpdates();
      console.log('=== IPC UPDATER: Manual update check completed:', result);
      return result;
    } catch (error) {
      console.error('=== IPC UPDATER: Error in manual update check:', error);
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

  // Debug helper for testing GitHub API access
  ipcMain.handle('updater:testGitHubAPI', async () => {
    try {
      console.log('=== TEST API: Testing GitHub API access...');
      const https = require('https');
      
      const result = await new Promise((resolve, reject) => {
        const req = https.request('https://api.github.com/repos/JoshuaCarroll/BasketballRosterManager/releases/latest', {
          method: 'GET',
          timeout: 15000,
          headers: {
            'User-Agent': 'BasketballRosterManager-Updater/3.0.1'
          }
        }, (res) => {
          let data = '';
          res.on('data', (chunk) => data += chunk);
          res.on('end', () => {
            try {
              const parsed = JSON.parse(data);
              console.log('=== TEST API: GitHub API response:', {
                statusCode: res.statusCode,
                latestVersion: parsed.tag_name,
                publishedAt: parsed.published_at,
                assetsCount: parsed.assets ? parsed.assets.length : 0
              });
              resolve({
                success: true,
                statusCode: res.statusCode,
                latestVersion: parsed.tag_name,
                publishedAt: parsed.published_at,
                assets: parsed.assets ? parsed.assets.map(a => ({ name: a.name, size: a.size, downloadUrl: a.browser_download_url })) : []
              });
            } catch (parseError) {
              console.error('=== TEST API: Parse error:', parseError.message);
              resolve({ success: false, error: 'Parse error: ' + parseError.message, rawData: data });
            }
          });
        });
        
        req.on('error', (err) => {
          console.error('=== TEST API: Request error:', err.message);
          reject(err);
        });
        
        req.on('timeout', () => {
          console.error('=== TEST API: Request timeout');
          req.destroy();
          reject(new Error('Request timeout'));
        });
        
        req.end();
      });
      
      return result;
    } catch (error) {
      console.error('=== TEST API: Error testing GitHub API:', error);
      return { success: false, error: error.message };
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