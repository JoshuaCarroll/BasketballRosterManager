const { contextBridge, ipcRenderer } = require('electron');

console.log('Preload script starting...');

try {
  console.log('Setting up electronAPI...');
  
  // Expose protected methods that allow the renderer process to use
  // the ipcRenderer without exposing the entire object
  contextBridge.exposeInMainWorld('electronAPI', {
    // Database operations
    getLeagues: () => ipcRenderer.invoke('db:getLeagues'),
    getTeams: (leagueId) => ipcRenderer.invoke('db:getTeams', leagueId),
    getPlayers: (teamId) => ipcRenderer.invoke('db:getPlayers', teamId),
    
    createLeague: (name, foulResetPeriod, bonusFouls, doubleBonusFouls, maxPlayerFouls) => 
      ipcRenderer.invoke('db:createLeague', name, foulResetPeriod, bonusFouls, doubleBonusFouls, maxPlayerFouls),
    createTeam: (leagueId, name, color) => 
      ipcRenderer.invoke('db:createTeam', leagueId, name, color),
    createPlayer: (teamId, jerseyNumber, name, description) =>
      ipcRenderer.invoke('db:createPlayer', teamId, jerseyNumber, name, description),
      
    updatePlayer: (playerId, jerseyNumber, name, description) =>
      ipcRenderer.invoke('db:updatePlayer', playerId, jerseyNumber, name, description),
    updateLeague: (leagueId, name, foulResetPeriod, bonusFouls, doubleBonusFouls, maxPlayerFouls) =>
      ipcRenderer.invoke('db:updateLeague', leagueId, name, foulResetPeriod, bonusFouls, doubleBonusFouls, maxPlayerFouls),
    updateTeam: (teamId, name, color) =>
      ipcRenderer.invoke('db:updateTeam', teamId, name, color),
      
    deletePlayer: (playerId) => ipcRenderer.invoke('db:deletePlayer', playerId),
    deleteAllPlayersForTeam: (teamId) => ipcRenderer.invoke('db:deleteAllPlayersForTeam', teamId),
    deleteLeague: (leagueId) => ipcRenderer.invoke('db:deleteLeague', leagueId),
    deleteTeam: (teamId) => ipcRenderer.invoke('db:deleteTeam', teamId),

    // Settings
    getSetting: (key) => ipcRenderer.invoke('settings:get', key),
    setSetting: (key, value) => ipcRenderer.invoke('settings:set', key, value),
    updateMenuPointColumns: (checked) => ipcRenderer.invoke('menu:updatePointColumns', checked),
    
    // Auto-updater functions
    checkForUpdates: () => ipcRenderer.invoke('updater:checkForUpdates'),
    downloadUpdate: () => ipcRenderer.invoke('updater:downloadUpdate'),
    quitAndInstall: () => ipcRenderer.invoke('updater:quitAndInstall'),
    testGitHubAPI: () => ipcRenderer.invoke('updater:testGitHubAPI'),
    
    // Update progress listener
    onUpdateProgress: (callback) => {
      ipcRenderer.on('update-progress', callback);
    },
    
    removeUpdateProgressListener: () => {
      ipcRenderer.removeAllListeners('update-progress');
    },
    
    // Menu actions
    onMenuAction: (callback) => {
      ipcRenderer.on('menu-action', callback);
    },
    
    removeMenuListener: () => {
      ipcRenderer.removeAllListeners('menu-action');
    }
  });
  
  console.log('ElectronAPI exposed successfully');
} catch (error) {
  console.error('Error in preload script:', error);
}