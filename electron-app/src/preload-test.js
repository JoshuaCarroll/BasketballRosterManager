const { contextBridge } = require('electron');

console.log('=== PRELOAD TEST: Starting ===');
console.log('=== PRELOAD TEST: contextBridge available:', !!contextBridge);

try {
  contextBridge.exposeInMainWorld('testAPI', {
    test: () => 'Hello from preload!'
  });
  console.log('=== PRELOAD TEST: Successfully exposed testAPI ===');
} catch (error) {
  console.error('=== PRELOAD TEST: Error exposing API:', error);
}

console.log('=== PRELOAD TEST: Script complete ===');