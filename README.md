# Basketball Roster Manager v3.0 - Cross-Platform Edition

As a public address announcer I've announced nearly 1,000 games varying from club leagues, to varsity, to NCAA Division I to MiLB. I know that rosters are valuable. I also know just how easy it is to get rosters mixed up. At JV and varsity games, some coaches list players by jersey number (bless you) and some alphabetically (go stub your toe!), while sometimes all you get is a scribbled piece of paper 2 minutes before the game in no order whatsoever! (Seriously, who raised you!?)

I created this program to help keep up with rosters, but it quickly turned into something so much more. Now it tracks personal fouls, team fouls, field goals, three pointers, and free throws! Rosters are automatically sorted numerically by jersey number.  They can also be color-coded to match the teams' jerseys. Lastly, you can easily swap the rosters from one side of the screen to the other, enabling you to keep the roster on the same side as their goal.

Basketball Roster Manager is now cross platform!  So whether you're using Windows or Mac, you're ready to roll!

![capture2](https://github.com/user-attachments/assets/4739a420-2469-40d8-b485-aa25790ee0b4)

## Getting Started

Download the run the latest release on the [releases page](https://github.com/JoshuaCarroll/BasketballRosterManager/releases/).

## 🎮 How to Use

### Quick Start
1. **Select a League** from the dropdown (determines foul rules)
2. **Create/Select Teams** for home and away sides
3. **Add Players** with jersey numbers and names
4. **Track the Game**:
   - Check players "in" when they enter the game
   - Double-click stats to increment (fouls, FG, 3P, FT)
   - Watch for automatic bonus/double-bonus indicators
   - Points calculate automatically

### Smart Features
- **Auto-Sorting**: Players stay sorted by number even after check-ins
- **Visual Feedback**: Checked-in players highlighted in yellow
- **Foul Tracking**: Adapts to your league's reset rules (quarter/half/game)
- **Team Swapping**: Easily switch home/away sides during games
- **Keyboard Shortcuts**: Use Ctrl+S to swap, Ctrl+R to reset fouls, etc.

## 🏀 League Presets Included

The app comes pre-loaded with common basketball league configurations:

- **NCAA Men/Women**: Fouls reset at half, bonus at 7, double-bonus at 10
- **High School Boys**: Fouls reset per quarter, bonus at 7, double-bonus at 10  
- **High School Girls**: Fouls reset per quarter, double-bonus at 5 (no regular bonus)
- **Varsity Boys/Girls**: Fouls reset at half, bonus at 7, double-bonus at 10

You can also create custom leagues with your own foul rules!

## ✨ Key Improvements Over Original

### 🆕 New Features You Requested
- **✅ Unlimited Players**: No longer limited to 28 rows - add as many players as needed
- **✅ Smart Auto-Sorting**: Players automatically resort by jersey number even after check-ins
- **✅ Configurable Foul Rules**: Different leagues have different foul reset periods and bonus thresholds
- **✅ Quarter/Half Tracking**: Support for leagues that reset fouls each quarter vs. each half
- **✅ Enhanced Bonus System**: Visual indicators for bonus (e.g., 7 fouls) and double-bonus (e.g., 5 fouls for HS girls)

### 🌟 Additional Modern Enhancements
- **Cross-Platform**: Runs on Windows, macOS, and Linux
- **Modern UI**: Clean, responsive design with CSS Grid
- **Double-Click Increments**: Double-click any stat to add +1
- **Team Colors**: Visual team identification 
- **Keyboard Shortcuts**: Quick access to common functions
- **Better Database**: SQLite for reliable local storage
- **Electron Packaging**: Self-contained executables

## Contributing 

Contributions to this project can be made by using the program and providing feedback or by assisting in the programming and submitting pull requests. If you have a feature request or find a problem, simply report it on the [issues tab](https://github.com/JoshuaCarroll/BasketballRosterManager/issues) tab.

## 🚀 Installation & Setup

### Option 1: Development Mode (Recommended for Testing)
```bash
cd electron-app
npm install        # (already done)
npm start          # Start the app
```

### Option 2: Build Distributable Version
```bash
cd electron-app
npm run build      # Builds for your current platform
npm run build:win  # Windows executable + installer
npm run build:mac  # macOS .dmg file
```

The built apps will be in the `dist/` folder and can be distributed without Node.js.

## 📁 Project Structure

```
electron-app/
├── src/
│   ├── main.js          # Electron main process (database & app logic)
│   ├── preload.js       # Security bridge between main/renderer
│   ├── index.html       # Main UI structure
│   ├── js/app.js        # Frontend JavaScript logic
│   └── styles/          # Modern CSS with CSS Grid
├── assets/              # Icons and images
├── package.json         # Dependencies and build scripts
└── README.md           # Detailed documentation
```

## 🔧 Technical Stack

- **Electron**: Cross-platform desktop framework
- **SQLite**: Local database (no server needed)
- **Modern CSS**: CSS Grid, Flexbox, CSS Variables for theming
- **Vanilla JavaScript**: No framework dependencies - fast and reliable
- **Self-Contained**: Everything bundled - no internet required

## 🎯 Comparison with Original

| Feature | Original (2014) | New Version (2024) |
|---------|-----------------|-------------------|
| Platform | Windows only | Windows, macOS, Linux |
| UI Framework | Windows Forms | Modern web UI |
| Player Limit | 28 rows | Unlimited |
| Sorting | Static | Dynamic by jersey # |
| Foul Rules | Fixed | Configurable by league |
| Database | SQL Server CE | SQLite |
| Period Tracking | Half only | Quarter or Half |
| Bonus System | Basic | Visual indicators |
| Distribution | .NET installer | Self-contained executable |

## 🏆 Ready to Use!

The application is **running successfully** and ready for use! The database has been initialized with sample leagues, and you can start adding teams and players right away.

**What's Next:**
1. Try creating a new league with custom foul rules
2. Add a couple test teams with different colors
3. Add some players and test the game tracking features
4. Use the double-click functionality for quick stat updates
5. Test the team swapping and foul reset features

This modern version maintains all the workflow efficiency of your original application while adding the cross-platform compatibility and enhanced features you requested!

---

*Note: The application automatically creates its database in your user data folder, so all your leagues, teams, and players will persist between sessions.*
