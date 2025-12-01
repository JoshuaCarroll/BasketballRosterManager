# Basketball Roster Manager - Electron Edition

A modern, cross-platform Basketball Roster Manager built with Electron, designed for public address announcers during basketball games.

## Features

### New in Version 3.0

- **Cross-platform compatibility** - Runs on Windows, macOS, and Linux
- **Modern UI** - Clean, responsive design with dark/light theme support
- **Unlimited players** - No longer limited to 28 rows, add as many players as needed
- **Smart sorting** - Players automatically sorted by jersey number, even after check-ins
- **Configurable foul rules** - Different leagues have different foul reset periods and bonus rules:
  - **NCAA**: Fouls reset at half, bonus at 7, double-bonus at 10
  - **High School Girls**: Fouls reset each quarter, double-bonus at 5
  - **Custom leagues**: Configure your own foul rules
- **Quarter/Half tracking** - Track fouls by quarter for leagues that reset each quarter
- **Enhanced statistics** - Track field goals, 3-pointers, free throws, and total points
- **Team colors** - Visual team identification with customizable colors
- **Keyboard shortcuts** - Quick access to common functions
- **Double-click increments** - Double-click any stat field to add 1

### Core Features

- **Roster Management**: Add, edit, delete players with jersey numbers and names
- **Game Statistics**: Track fouls, shooting statistics, and points
- **Player Check-in**: Mark players as "in the game" for announcement tracking
- **Team Swapping**: Easily swap teams between home/away sides
- **Foul Tracking**: Visual indicators for bonus and double-bonus situations
- **League Management**: Create custom leagues with specific rules
- **Team Management**: Create teams with custom colors
- **Data Persistence**: All data saved locally in SQLite database

## Installation

### Prerequisites

- Node.js 18 or higher
- npm or yarn package manager

### Development Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/JoshuaCarroll/BasketballRosterManager.git
   cd BasketballRosterManager/electron-app
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Run in development mode**
   ```bash
   npm run dev
   ```

### Building for Production

1. **Install dependencies** (if not already done)
   ```bash
   npm install
   ```

2. **Build for all platforms**
   ```bash
   npm run build
   ```

3. **Build for specific platforms**
   ```bash
   # Windows
   npm run build:win
   
   # macOS
   npm run build:mac
   
   # Linux
   npm run build:linux
   ```

4. **Create distributable packages**
   ```bash
   npm run dist
   ```

The built applications will be available in the `dist` folder:
- **Windows**: `.exe` installer and portable `.exe` 
- **macOS**: `.dmg` disk image
- **Linux**: `.AppImage` executable

### Installation from Built Package

#### Windows
1. Download the `.exe` installer from the releases page
2. Run the installer and follow the prompts
3. Or use the portable version that doesn't require installation

#### macOS
1. Download the `.dmg` file from the releases page
2. Open the `.dmg` file
3. Drag the Basketball Roster Manager app to your Applications folder

#### Linux
1. Download the `.AppImage` file from the releases page
2. Make it executable: `chmod +x Basketball-Roster-Manager-*.AppImage`
3. Run the application: `./Basketball-Roster-Manager-*.AppImage`

## Usage

### Getting Started

1. **Create a League**
   - Click "New League" in the header
   - Set the league name and foul rules
   - Common presets are included for NCAA and High School

2. **Create Teams**
   - Select a league from the dropdown
   - Click "New Team" for either home or away
   - Set team name and choose team colors

3. **Add Players**
   - Select teams for both home and away
   - Click "Add Player" for each team
   - Enter jersey number, player name, and optional graduating class

4. **Track the Game**
   - Use checkboxes to mark players as "in the game"
   - Double-click stat fields to increment (fouls, FG, 3P, FT)
   - Watch for bonus/double-bonus indicators
   - Use period selector to track current game period

### Keyboard Shortcuts

- `Ctrl+Shift+L` (Cmd+Shift+L on Mac): New League
- `Ctrl+Shift+T` (Cmd+Shift+T on Mac): New Team  
- `Ctrl+N` (Cmd+N on Mac): New Game (reset stats)
- `Ctrl+R` (Cmd+R on Mac): Reset Fouls
- `Ctrl+S` (Cmd+S on Mac): Swap Teams

### Tips

- **Double-click any stat field** to quickly add 1 to the current value
- **Players are automatically sorted** by jersey number for easy finding
- **Team colors** help visually distinguish teams during fast-paced games
- **Bonus indicators** automatically appear based on team foul counts
- **Foul tracking adapts** to your league's reset rules (quarter, half, or game)

## Database

The application uses SQLite for local data storage. The database file is automatically created in your user data directory:

- **Windows**: `%APPDATA%\basketball-roster-manager\basketball-roster.db`
- **macOS**: `~/Library/Application Support/basketball-roster-manager/basketball-roster.db`
- **Linux**: `~/.config/basketball-roster-manager/basketball-roster.db`

## Development

### Project Structure

```
electron-app/
├── src/
│   ├── main.js          # Electron main process
│   ├── preload.js       # Preload script for security
│   ├── index.html       # Main application HTML
│   ├── js/
│   │   └── app.js       # Frontend application logic
│   └── styles/
│       ├── main.css     # Main stylesheet
│       └── components.css # Component styles
├── assets/              # Application icons
├── package.json         # Dependencies and build config
└── README.md           # This file
```

### Technologies Used

- **Electron**: Cross-platform desktop framework
- **SQLite**: Local database with better-sqlite3
- **HTML5/CSS3**: Modern web standards
- **JavaScript ES6+**: Modern JavaScript features
- **CSS Grid/Flexbox**: Responsive layouts
- **CSS Custom Properties**: Theming system

### Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature-name`
3. Make your changes
4. Test thoroughly on your target platforms
5. Commit your changes: `git commit -am 'Add feature'`
6. Push to the branch: `git push origin feature-name`
7. Create a Pull Request

## License

MIT License - see the original repository for full license details.

## Support

For issues, feature requests, or questions:
- Create an issue on GitHub
- Check existing issues for similar problems
- Provide detailed information about your system and the issue

## Changelog

### Version 3.0.0
- Complete rewrite in Electron for cross-platform support
- Modern UI with responsive design
- Unlimited player capacity
- Configurable league foul rules
- Enhanced statistics tracking
- Quarter-based foul tracking
- Improved sorting and player management