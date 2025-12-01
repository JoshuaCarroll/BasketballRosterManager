// Minimal preload script - no dependencies, just basic test
console.log('MINIMAL PRELOAD: Script is running!');

try {
  // Test if we can access basic Node.js APIs
  console.log('MINIMAL PRELOAD: process.versions:', JSON.stringify(process.versions));
  
  // Test if require works
  const { contextBridge } = require('electron');
  console.log('MINIMAL PRELOAD: contextBridge loaded successfully');
  
  // Test if contextBridge works
  contextBridge.exposeInMainWorld('simpleTest', {
    hello: 'world',
    getVersion: () => process.versions.electron
  });
  
  console.log('MINIMAL PRELOAD: Successfully exposed simpleTest to main world');
} catch (error) {
  console.error('MINIMAL PRELOAD: Error occurred:', error.message);
  console.error('MINIMAL PRELOAD: Stack trace:', error.stack);
}