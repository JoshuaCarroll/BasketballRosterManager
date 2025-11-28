# Quick Setup Guide

## For Development/Testing

1. **Install Node.js** (if not already installed)
   - Download from https://nodejs.org/
   - Choose the LTS version (18.x or higher)

2. **Open Terminal/PowerShell** in the `electron-app` folder

3. **Install dependencies**
   ```bash
   npm install
   ```

4. **Start the application**
   ```bash
   npm start
   ```

   Or for development with DevTools:
   ```bash
   npm run dev
   ```

## For Distribution

1. **Build the application**
   ```bash
   # For current platform only
   npm run build
   
   # For Windows specifically
   npm run build:win
   
   # For macOS specifically  
   npm run build:mac
   ```

2. **Find the built application**
   - Check the `dist` folder
   - Windows: Look for `.exe` files
   - macOS: Look for `.dmg` files
   - Linux: Look for `.AppImage` files

## Troubleshooting

### Common Issues

1. **"better-sqlite3" installation fails**
   - Make sure you have Python and Visual Studio Build Tools installed on Windows
   - Or use: `npm install --build-from-source`

2. **Permission errors on macOS/Linux**
   - Use `sudo npm install` if needed
   - Or use a Node version manager like nvm

3. **Application won't start**
   - Check the terminal for error messages
   - Make sure all dependencies installed correctly
   - Try deleting `node_modules` and running `npm install` again

### Performance Tips

- The database file is created automatically on first run
- Player data persists between sessions
- For large rosters (50+ players), consider using the search/filter features

### Platform-Specific Notes

**Windows:**
- Portable version doesn't require installation
- Database stored in `%APPDATA%\basketball-roster-manager\`

**macOS:**
- May need to allow app in Security & Privacy settings
- Database stored in `~/Library/Application Support/basketball-roster-manager/`

**Linux:**
- AppImage format runs without installation
- Database stored in `~/.config/basketball-roster-manager/`