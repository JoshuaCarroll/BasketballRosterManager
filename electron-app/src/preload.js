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
    
    createLeague: (name, foulResetPeriod, bonusFouls, doubleBonusFouls) => 
      ipcRenderer.invoke('db:createLeague', name, foulResetPeriod, bonusFouls, doubleBonusFouls),
    createTeam: (leagueId, name, color) => 
      ipcRenderer.invoke('db:createTeam', leagueId, name, color),
    createPlayer: (teamId, jerseyNumber, name, graduatingClass) =>
      ipcRenderer.invoke('db:createPlayer', teamId, jerseyNumber, name, graduatingClass),
      
    updatePlayer: (playerId, jerseyNumber, name, graduatingClass) =>
      ipcRenderer.invoke('db:updatePlayer', playerId, jerseyNumber, name, graduatingClass),
      
    deletePlayer: (playerId) => ipcRenderer.invoke('db:deletePlayer', playerId),
    
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