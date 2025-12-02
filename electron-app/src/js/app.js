// Basketball Roster Manager - Main Application Logic

class BasketballRosterManager {
  constructor() {
    this.currentLeague = null;
    this.homeTeam = null;
    this.awayTeam = null;
    this.currentPeriod = 1;
    this.teamsSwapped = false; // Track if teams are currently swapped
    this.gameStats = {
      home: {},
      away: {}
    };
    this.editingPlayer = null;
    this.editingTeam = null;
    this.editingLeague = null;
    this.editingTeamId = null;
    this.currentPlayers = { home: [], away: [] };

    this.init();
  }

  async init() {
    // Wait for electronAPI to be available
    await this.waitForElectronAPI();
    
    try {
      await this.loadLeagues();
      this.setupEventListeners();
      this.setupMenuHandlers();
    } catch (error) {
      console.error('Failed to initialize app:', error);
      this.showNotification('Failed to initialize application. Please check the console for details.', 'error');
    }
  }

  async waitForElectronAPI() {
    // Test if preload script is working at all
    console.log('=== RENDERER TEST: Checking for electronAPI:', !!window.electronAPI);
    if (window.electronAPI) {
      console.log('=== RENDERER TEST: electronAPI methods:', Object.keys(window.electronAPI));
    }
    
    // Wait for electronAPI to be available (max 5 seconds)
    const maxWait = 5000;
    const startTime = Date.now();
    
    console.log('Waiting for electronAPI...');
    
    while (!window.electronAPI && (Date.now() - startTime) < maxWait) {
      await new Promise(resolve => setTimeout(resolve, 100));
      console.log('Still waiting for electronAPI...', Date.now() - startTime, 'ms elapsed');
    }
    
    if (!window.electronAPI) {
      console.error('ElectronAPI failed to load. Possible causes:');
      console.error('1. Preload script failed to execute');
      console.error('2. Context isolation issues');
      console.error('3. File path problems');
      console.error('Current window properties:', Object.keys(window));
      
      // Create a mock API for development/debugging
      console.warn('Creating mock electronAPI for debugging purposes');
      window.electronAPI = this.createMockAPI();
      
      this.showNotification('Running in fallback mode - some features may not work', 'warning');
    } else {
      console.log('ElectronAPI is available with methods:', Object.keys(window.electronAPI));
    }
  }
  
  createMockAPI() {
    return {
      getLeagues: async () => {
        console.warn('Mock API: getLeagues called');
        return [
          { id: 1, name: 'Mock NCAA - Men', foul_reset_period: 'half', bonus_fouls: 7, double_bonus_fouls: 10 },
          { id: 2, name: 'Mock High School - Girls', foul_reset_period: 'quarter', bonus_fouls: 5, double_bonus_fouls: 5 }
        ];
      },
      getTeams: async (leagueId) => {
        console.warn('Mock API: getTeams called with leagueId:', leagueId);
        return [];
      },
      getPlayers: async (teamId) => {
        console.warn('Mock API: getPlayers called with teamId:', teamId);
        return [];
      },
      createLeague: async (...args) => {
        console.warn('Mock API: createLeague called with:', args);
        throw new Error('Mock API: Cannot create leagues in fallback mode');
      },
      createTeam: async (...args) => {
        console.warn('Mock API: createTeam called with:', args);
        throw new Error('Mock API: Cannot create teams in fallback mode');
      },
      createPlayer: async (...args) => {
        console.warn('Mock API: createPlayer called with:', args);
        throw new Error('Mock API: Cannot create players in fallback mode');
      },
      updatePlayer: async (...args) => {
        console.warn('Mock API: updatePlayer called with:', args);
        throw new Error('Mock API: Cannot update players in fallback mode');
      },
      deletePlayer: async (...args) => {
        console.warn('Mock API: deletePlayer called with:', args);
        throw new Error('Mock API: Cannot delete players in fallback mode');
      },
      onMenuAction: (callback) => {
        console.warn('Mock API: onMenuAction called');
      },
      removeMenuListener: () => {
        console.warn('Mock API: removeMenuListener called');
      }
    };
  }

  // ===== DATA LOADING =====
  async loadLeagues() {
    if (!window.electronAPI || !window.electronAPI.getLeagues) {
      throw new Error('ElectronAPI getLeagues method not available');
    }
    
    try {
      console.log('Loading leagues...');
      const leagues = await window.electronAPI.getLeagues();
      console.log('Leagues loaded:', leagues);
      
      const leagueSelect = document.getElementById('league-select');
      
      if (!leagueSelect) {
        throw new Error('League select element not found');
      }
      
      leagueSelect.innerHTML = '<option value="">Select League...</option>';
      
      leagues.forEach(league => {
        const option = document.createElement('option');
        option.value = league.id;
        option.textContent = league.name;
        option.dataset.foulResetPeriod = league.foul_reset_period;
        option.dataset.bonusFouls = league.bonus_fouls;
        option.dataset.doubleBonusFouls = league.double_bonus_fouls;
        leagueSelect.appendChild(option);
      });
      
      // Add create new league option
      const createOption = document.createElement('option');
      createOption.value = 'CREATE_NEW';
      createOption.textContent = '* Create new league';
      leagueSelect.appendChild(createOption);

      // Auto-select first league if available
      if (leagues.length > 0) {
        leagueSelect.selectedIndex = 1;
        await this.onLeagueChange();
      }
      
      console.log('Leagues loaded successfully');
    } catch (error) {
      console.error('Failed to load leagues:', error);
      this.showNotification('Failed to load leagues: ' + error.message, 'error');
      throw error;
    }
  }

  async loadTeams(leagueId) {
    try {
      const teams = await window.electronAPI.getTeams(leagueId);
      
      const homeSelect = document.getElementById('home-team-select');
      const awaySelect = document.getElementById('away-team-select');
      
      const homeDefaultOption = '<option value="">Select Home Team...</option>';
      const awayDefaultOption = '<option value="">Select Away Team...</option>';
      homeSelect.innerHTML = homeDefaultOption;
      awaySelect.innerHTML = awayDefaultOption;
      
      teams.forEach(team => {
        const option = `<option value="${team.id}" data-color="${team.color}">${team.name}</option>`;
        homeSelect.innerHTML += option;
        awaySelect.innerHTML += option;
      });
      
      // Add create new team options
      const createOption = '<option value="CREATE_NEW">* Create new team</option>';
      homeSelect.innerHTML += createOption;
      awaySelect.innerHTML += createOption;
    } catch (error) {
      console.error('Failed to load teams:', error);
      this.showNotification('Failed to load teams', 'error');
    }
  }

  async loadPlayers(teamId, isHome = true) {
    try {
      const players = await window.electronAPI.getPlayers(teamId);
      const container = document.getElementById(isHome ? 'home-players' : 'away-players');
      
      // Sort players by jersey number with custom logic
      players.sort((a, b) => {
        return this.compareJerseyNumbers(a.jersey_number, b.jersey_number);
      });

      container.innerHTML = '';

      if (players.length === 0) {
        container.innerHTML = `
          <div class="empty-state">
            <h3>No Players Added</h3>
            <p>Click "Add Player" to add players to this team.</p>
          </div>
        `;
        return;
      }

      // Store players data for editing
      const teamKey = isHome ? 'home' : 'away';
      this.currentPlayers[teamKey] = players;
      
      players.forEach(player => {
        this.createPlayerRow(player, isHome);
      });

      // Initialize game stats for this team
      this.gameStats[teamKey] = {};
      players.forEach(player => {
        this.gameStats[teamKey][player.id] = {
          fouls: [0, 0, 0, 0], // quarters 1-4
          fieldGoals: 0,
          threePointers: 0,
          freeThrows: 0,
          isCheckedIn: false
        };
      });

      this.updateTeamFoulDisplay(isHome);
    } catch (error) {
      console.error('Failed to load players:', error);
      this.showNotification('Failed to load players', 'error');
    }
  }

  // ===== UI CREATION =====
  createPlayerRow(player, isHome) {
    const container = document.getElementById(isHome ? 'home-players' : 'away-players');
    const teamKey = isHome ? 'home' : 'away';
    const stats = this.gameStats[teamKey][player.id] || {
      fouls: [0, 0, 0, 0],
      fieldGoals: 0,
      threePointers: 0,
      freeThrows: 0,
      isCheckedIn: false
    };

    const totalFouls = this.calculateTotalFouls(stats.fouls);
    const totalPoints = (stats.fieldGoals * 2) + (stats.threePointers * 3) + stats.freeThrows;

    const row = document.createElement('div');
    row.className = `player-row ${stats.isCheckedIn ? 'checked-in' : ''}`;
    row.dataset.playerId = player.id;
    row.dataset.teamSide = teamKey;

    row.innerHTML = `
      <input type="checkbox" class="player-checkbox" ${stats.isCheckedIn ? 'checked' : ''}>
      <div class="player-number">${player.jersey_number}</div>
      <div class="player-name" title="${player.name}">${player.name}${player.description ? ' (' + player.description + ')' : ''}</div>
      <div class="player-stat foul-count ${this.getFoulClass(totalFouls)}">
        <input type="number" class="stat-input foul-input" value="${totalFouls}" min="0" max="20">
      </div>
      <div class="player-stat">
        <input type="number" class="stat-input" value="${stats.fieldGoals}" min="0" data-stat="fieldGoals">
      </div>
      <div class="player-stat">
        <input type="number" class="stat-input" value="${stats.threePointers}" min="0" data-stat="threePointers">
      </div>
      <div class="player-stat">
        <input type="number" class="stat-input" value="${stats.freeThrows}" min="0" data-stat="freeThrows">
      </div>
      <div class="player-stat">
        <input type="number" class="stat-input stat-calculated" value="${totalPoints}" readonly>
      </div>
      <div class="player-actions">
        <button class="action-btn edit-btn" data-action="edit" data-player-id="${player.id}">Edit</button>
        <button class="action-btn delete-btn" data-action="delete" data-player-id="${player.id}">Delete</button>
      </div>
    `;

    container.appendChild(row);
    this.setupPlayerRowEvents(row);
  }

  setupPlayerRowEvents(row) {
    const checkbox = row.querySelector('.player-checkbox');
    const statInputs = row.querySelectorAll('.stat-input[data-stat]');
    const foulInput = row.querySelector('.foul-input');

    // Checkbox for player check-in
    checkbox.addEventListener('change', (e) => {
      const playerId = parseInt(row.dataset.playerId);
      const teamKey = row.dataset.teamSide;
      const isChecked = e.target.checked;

      this.gameStats[teamKey][playerId].isCheckedIn = isChecked;
      row.classList.toggle('checked-in', isChecked);
    });

    // Stat inputs
    statInputs.forEach(input => {
      input.addEventListener('change', (e) => {
        const playerId = parseInt(row.dataset.playerId);
        const teamKey = row.dataset.teamSide;
        const statType = e.target.dataset.stat;
        const value = parseInt(e.target.value) || 0;

        this.gameStats[teamKey][playerId][statType] = value;
        this.updatePlayerTotals(row);
      });

      // Double-click to increment
      input.addEventListener('dblclick', (e) => {
        const currentValue = parseInt(e.target.value) || 0;
        e.target.value = currentValue + 1;
        e.target.dispatchEvent(new Event('change'));
      });
    });

    // Foul input - double-click to add foul, change to set foul manually
    foulInput.addEventListener('dblclick', (e) => {
      const playerId = parseInt(row.dataset.playerId);
      const teamKey = row.dataset.teamSide;
      
      this.addFoul(playerId, teamKey);
      this.updatePlayerFoulDisplay(row);
      this.updateTeamFoulDisplay(teamKey === 'home');
    });

    // Foul input - manual change event
    foulInput.addEventListener('change', (e) => {
      const playerId = parseInt(row.dataset.playerId);
      const teamKey = row.dataset.teamSide;
      const newFoulCount = parseInt(e.target.value) || 0;
      
      this.setPlayerFouls(playerId, teamKey, newFoulCount);
      this.updatePlayerFoulDisplay(row);
      this.updateTeamFoulDisplay(teamKey === 'home');
    });
  }

  updatePlayerTotals(row) {
    const statInputs = row.querySelectorAll('.stat-input[data-stat]');
    const pointsInput = row.querySelector('.stat-calculated');
    
    let fieldGoals = 0, threePointers = 0, freeThrows = 0;
    
    statInputs.forEach(input => {
      const value = parseInt(input.value) || 0;
      switch (input.dataset.stat) {
        case 'fieldGoals': fieldGoals = value; break;
        case 'threePointers': threePointers = value; break;
        case 'freeThrows': freeThrows = value; break;
      }
    });

    const totalPoints = (fieldGoals * 2) + (threePointers * 3) + freeThrows;
    pointsInput.value = totalPoints;
  }

  // ===== FOUL MANAGEMENT =====
  addFoul(playerId, teamKey) {
    const stats = this.gameStats[teamKey][playerId];
    const periodIndex = this.currentPeriod - 1;
    
    if (periodIndex >= 0 && periodIndex < 4) {
      stats.fouls[periodIndex]++;
    }
  }

  setPlayerFouls(playerId, teamKey, totalFouls) {
    const stats = this.gameStats[teamKey][playerId];
    const currentPeriodIndex = this.currentPeriod - 1;
    
    // Ensure totalFouls is non-negative
    totalFouls = Math.max(0, totalFouls);
    
    // Reset all fouls to 0 first
    stats.fouls = [0, 0, 0, 0];
    
    // Put all fouls in the current period for simplicity
    // This approach makes it easy to track manual edits
    if (currentPeriodIndex >= 0 && currentPeriodIndex < 4) {
      stats.fouls[currentPeriodIndex] = totalFouls;
    } else {
      // If no valid period, put fouls in period 1
      stats.fouls[0] = totalFouls;
    }
  }

  calculateTotalFouls(foulArray) {
    // Individual player fouls are always cumulative (never reset)
    return foulArray.reduce((sum, fouls) => sum + fouls, 0);
  }

  compareJerseyNumbers(jerseyA, jerseyB) {
    // Custom sorting for jersey numbers: 0, 00, 1, 2, 3, 19, 22, HC
    // Numbers first (with special handling for 0 vs 00), then alphabetic
    
    const isNumericA = /^\d+$/.test(jerseyA);
    const isNumericB = /^\d+$/.test(jerseyB);
    
    // If both are numeric
    if (isNumericA && isNumericB) {
      const numA = parseInt(jerseyA);
      const numB = parseInt(jerseyB);
      
      // Handle special case: 0 should come before 00
      if (numA === 0 && numB === 0) {
        // If both parse to 0, sort by string length (shorter first)
        // "0" (length 1) comes before "00" (length 2)
        return jerseyA.length - jerseyB.length;
      }
      
      // Normal numeric sort
      return numA - numB;
    }
    
    // If one is numeric and one is not, numeric comes first
    if (isNumericA && !isNumericB) return -1;
    if (!isNumericA && isNumericB) return 1;
    
    // If both are non-numeric, sort alphabetically
    return jerseyA.localeCompare(jerseyB);
  }

  calculateTeamFoulsForPeriod(foulArray) {
    // Team fouls reset based on league rules
    const league = this.getCurrentLeague();
    if (!league) return foulArray.reduce((sum, fouls) => sum + fouls, 0);

    switch (league.foul_reset_period) {
      case 'quarter':
        return foulArray[this.currentPeriod - 1] || 0;
      case 'half':
        const firstHalf = (foulArray[0] || 0) + (foulArray[1] || 0);
        const secondHalf = (foulArray[2] || 0) + (foulArray[3] || 0);
        return this.currentPeriod <= 2 ? firstHalf : secondHalf;
      case 'game':
      default:
        return foulArray.reduce((sum, fouls) => sum + fouls, 0);
    }
  }

  calculateTeamFouls(teamKey) {
    const teamStats = this.gameStats[teamKey];
    let totalFouls = 0;

    Object.values(teamStats).forEach(playerStats => {
      totalFouls += this.calculateTeamFoulsForPeriod(playerStats.fouls);
    });

    return totalFouls;
  }

  updatePlayerFoulDisplay(row) {
    const playerId = parseInt(row.dataset.playerId);
    const teamKey = row.dataset.teamSide;
    const stats = this.gameStats[teamKey][playerId];
    const totalFouls = this.calculateTotalFouls(stats.fouls);
    
    const foulInput = row.querySelector('.foul-input');
    const foulContainer = row.querySelector('.foul-count');
    
    foulInput.value = totalFouls;
    foulContainer.className = `player-stat foul-count ${this.getFoulClass(totalFouls)}`;
  }

  updateTeamFoulDisplay(isHome) {
    const teamKey = isHome ? 'home' : 'away';
    const teamFouls = this.calculateTeamFouls(teamKey);
    const league = this.getCurrentLeague();
    
    const foulElement = document.getElementById(`${teamKey}-team-fouls`);
    const bonusElement = document.getElementById(`${teamKey}-bonus-indicator`);
    
    foulElement.textContent = teamFouls;
    
    if (league) {
      bonusElement.className = 'bonus-indicator';
      bonusElement.textContent = '';
      
      if (teamFouls >= league.double_bonus_fouls) {
        bonusElement.className += ' double-bonus';
        bonusElement.textContent = 'Double Bonus';
      } else if (teamFouls >= league.bonus_fouls) {
        bonusElement.className += ' bonus';
        bonusElement.textContent = 'Bonus';
      }
    }
  }

  getFoulClass(foulCount) {
    if (foulCount >= 4) return 'high-fouls';
    if (foulCount >= 3) return 'warning-fouls';
    return '';
  }

  // ===== EVENT HANDLERS =====
  setupEventListeners() {
    // League selection
    document.getElementById('league-select').addEventListener('change', () => this.onLeagueChange());
    
    // Team selections
    document.getElementById('home-team-select').addEventListener('change', () => this.onTeamChange(true));
    document.getElementById('away-team-select').addEventListener('change', () => this.onTeamChange(false));
    
    // Period selection
    document.getElementById('period-select').addEventListener('change', (e) => {
      this.currentPeriod = parseInt(e.target.value);
      this.updateAllFoulDisplays();
    });
    
    // Modal controls
    this.setupModalControls();
    
    // Action buttons
    // New league and team options are now in the dropdowns
    document.getElementById('swap-teams-btn').addEventListener('click', () => this.swapTeams());
    document.getElementById('add-home-player-btn').addEventListener('click', () => this.showAddPlayerModal(true));
    document.getElementById('add-away-player-btn').addEventListener('click', () => this.showAddPlayerModal(false));
    
    // Edit buttons
    document.getElementById('edit-league-btn').addEventListener('click', () => this.editLeague());
    document.getElementById('edit-home-team-btn').addEventListener('click', () => this.editTeam(this.homeTeam));
    document.getElementById('edit-away-team-btn').addEventListener('click', () => this.editTeam(this.awayTeam));
    
    // Player action buttons (edit/delete) using event delegation
    document.addEventListener('click', (e) => {
      if (e.target.matches('.action-btn[data-action]')) {
        const action = e.target.dataset.action;
        const playerId = e.target.dataset.playerId;
        
        if (action === 'edit') {
          this.editPlayer(playerId);
        } else if (action === 'delete') {
          this.deletePlayer(playerId);
        }
      }
    });
  }

  setupMenuHandlers() {
    if (!window.electronAPI || !window.electronAPI.onMenuAction) {
      console.warn('ElectronAPI menu handler not available');
      return;
    }
    
    try {
      window.electronAPI.onMenuAction((event, action) => {
        switch (action) {
          case 'new-league':
            this.showNewLeagueModal();
            break;
          case 'new-team':
            this.showModal('new-team-modal');
            break;
          case 'new-game':
            this.resetGame();
            break;
          case 'reset-fouls':
            // Individual player fouls are never reset during a game
            // Team fouls automatically reset based on league rules
            this.showNotification('Individual player fouls are not reset during games. Team fouls reset automatically based on league rules.', 'info');
            break;
          case 'swap-teams':
            this.swapTeams();
            break;
        }
      });
    } catch (error) {
      console.error('Failed to setup menu handlers:', error);
    }
  }

  async onLeagueChange() {
    const leagueSelect = document.getElementById('league-select');
    const selectedOption = leagueSelect.options[leagueSelect.selectedIndex];
    const editLeagueBtn = document.getElementById('edit-league-btn');
    
    if (!selectedOption || !selectedOption.value) {
      this.currentLeague = null;
      editLeagueBtn.style.display = 'none';
      return;
    }
    
    // Handle create new league option
    if (selectedOption.value === 'CREATE_NEW') {
      // Reset selection to default
      leagueSelect.value = '';
      this.currentLeague = null;
      editLeagueBtn.style.display = 'none';
      // Show new league modal
      this.showNewLeagueModal();
      return;
    }

    this.currentLeague = {
      id: parseInt(selectedOption.value),
      name: selectedOption.textContent,
      foul_reset_period: selectedOption.dataset.foulResetPeriod,
      bonus_fouls: parseInt(selectedOption.dataset.bonusFouls),
      double_bonus_fouls: parseInt(selectedOption.dataset.doubleBonusFouls)
    };

    // Show edit league button
    editLeagueBtn.style.display = 'inline-block';

    // Update period options based on league
    this.updatePeriodOptions();
    
    // Load teams for this league
    await this.loadTeams(this.currentLeague.id);
    
    // Clear team selections and hide edit buttons
    document.getElementById('home-team-select').value = '';
    document.getElementById('away-team-select').value = '';
    document.getElementById('edit-home-team-btn').style.display = 'none';
    document.getElementById('edit-away-team-btn').style.display = 'none';
    this.clearRosters();
  }

  async onTeamChange(isHome) {
    const selectId = isHome ? 'home-team-select' : 'away-team-select';
    const select = document.getElementById(selectId);
    const selectedOption = select.options[select.selectedIndex];
    const editBtnId = isHome ? 'edit-home-team-btn' : 'edit-away-team-btn';
    const editBtn = document.getElementById(editBtnId);
    
    if (!selectedOption || !selectedOption.value) {
      editBtn.style.display = 'none';
      this.clearRosterForTeam(isHome);
      return;
    }
    
    // Handle create new team option
    if (selectedOption.value === 'CREATE_NEW') {
      // Reset selection to default
      select.value = '';
      editBtn.style.display = 'none';
      this.clearRosterForTeam(isHome);
      // Set current team side for the modal
      this.currentTeamSide = isHome;
      // Show new team modal
      this.showNewTeamModal();
      return;
    }

    const teamId = parseInt(selectedOption.value);
    const teamName = selectedOption.textContent;
    const teamColor = selectedOption.dataset.color;
    
    // Update team info
    if (isHome) {
      this.homeTeam = { id: teamId, name: teamName, color: teamColor };
    } else {
      this.awayTeam = { id: teamId, name: teamName, color: teamColor };
    }
    
    // Show edit button
    editBtn.style.display = 'inline-block';
    
    // Update roster header background color
    const rosterHeaderId = isHome ? 'home-roster' : 'away-roster';
    const rosterHeader = document.getElementById(rosterHeaderId).querySelector('.roster-header');
    rosterHeader.style.backgroundColor = teamColor;
    
    // Load players
    await this.loadPlayers(teamId, isHome);
  }

  // ===== MODAL MANAGEMENT =====
  setupModalControls() {
    const overlay = document.getElementById('modal-overlay');
    
    // Close modal when clicking overlay
    overlay.addEventListener('click', (e) => {
      if (e.target === overlay) {
        this.hideModal();
      }
    });

    // Close buttons
    document.querySelectorAll('.modal-close').forEach(btn => {
      btn.addEventListener('click', () => this.hideModal());
    });

    // Cancel buttons
    document.getElementById('cancel-league-btn').addEventListener('click', () => this.hideModal());
    document.getElementById('cancel-team-btn').addEventListener('click', () => this.hideModal());
    document.getElementById('cancel-player-btn').addEventListener('click', () => this.hideModal());

    // Save buttons
    document.getElementById('save-league-btn').addEventListener('click', () => this.saveLeague());
    document.getElementById('save-team-btn').addEventListener('click', () => this.saveTeam());
    document.getElementById('save-player-btn').addEventListener('click', () => this.savePlayer());
    
    // Add Enter key support for player modal
    document.getElementById('new-player-modal').addEventListener('keydown', (e) => {
      if (e.key === 'Enter') {
        e.preventDefault();
        this.savePlayer();
      }
    });

    // Color picker - initialize and add event listener
    const colorInput = document.getElementById('team-color');
    const colorPreview = document.getElementById('team-color-preview');
    
    // Initialize preview color
    colorPreview.style.backgroundColor = colorInput.value;
    
    colorInput.addEventListener('change', (e) => {
      colorPreview.style.backgroundColor = e.target.value;
    });
  }

  showModal(modalId) {
    console.log('showModal called with:', modalId);
    
    const overlay = document.getElementById('modal-overlay');
    const targetModal = document.getElementById(modalId);
    
    console.log('Modal overlay found:', !!overlay);
    console.log('Target modal found:', !!targetModal);
    
    if (!overlay || !targetModal) {
      console.error('Modal elements not found');
      return;
    }
    
    overlay.classList.remove('hidden');
    document.querySelectorAll('.modal').forEach(modal => modal.style.display = 'none');
    targetModal.style.display = 'block';
    
    console.log('Modal should be visible now');
    
    // Focus first input
    const firstInput = document.querySelector(`#${modalId} input, #${modalId} select`);
    if (firstInput) {
      setTimeout(() => firstInput.focus(), 100);
    }
  }

  hideModal() {
    document.getElementById('modal-overlay').classList.add('hidden');
    this.editingPlayer = null;
    this.editingTeam = null;
    
    // Clear form fields
    document.querySelectorAll('.modal input[type="text"], .modal input[type="number"]').forEach(input => {
      input.value = '';
    });
  }

  // ===== SAVE OPERATIONS =====
  async saveLeague() {
    const name = document.getElementById('league-name').value.trim();
    const foulResetPeriod = document.getElementById('foul-reset-period').value;
    const bonusFouls = parseInt(document.getElementById('bonus-fouls').value);
    const doubleBonusFouls = parseInt(document.getElementById('double-bonus-fouls').value);

    if (!name) {
      this.showNotification('Please enter a league name', 'error');
      return;
    }

    try {
      if (this.editingLeague) {
        // Update existing league
        await window.electronAPI.updateLeague(this.editingLeague, name, foulResetPeriod, bonusFouls, doubleBonusFouls);
        this.showNotification('League updated successfully', 'success');
        this.editingLeague = null;
      } else {
        // Create new league
        await window.electronAPI.createLeague(name, foulResetPeriod, bonusFouls, doubleBonusFouls);
        this.showNotification('League created successfully', 'success');
      }
      
      await this.loadLeagues();
      this.hideModal();
      
      // Reset modal for next use
      document.getElementById('league-modal-title').textContent = 'New League';
      document.getElementById('save-league-btn').textContent = 'Create League';
    } catch (error) {
      console.error('Failed to save league:', error);
      this.showNotification('Failed to save league', 'error');
    }
  }

  async saveTeam() {
    const name = document.getElementById('team-name').value.trim();
    const color = document.getElementById('team-color').value;

    if (!name) {
      this.showNotification('Please enter a team name', 'error');
      return;
    }

    if (!this.currentLeague) {
      this.showNotification('Please select a league first', 'error');
      return;
    }

    try {
      if (this.editingTeamId) {
        // Update existing team
        await window.electronAPI.updateTeam(this.editingTeamId, name, color);
        
        // Update local team data if it's currently selected
        if (this.homeTeam && this.homeTeam.id === this.editingTeamId) {
          this.homeTeam.name = name;
          this.homeTeam.color = color;
          const homeRosterHeader = document.getElementById('home-roster').querySelector('.roster-header');
          homeRosterHeader.style.backgroundColor = color;
        }
        if (this.awayTeam && this.awayTeam.id === this.editingTeamId) {
          this.awayTeam.name = name;
          this.awayTeam.color = color;
          const awayRosterHeader = document.getElementById('away-roster').querySelector('.roster-header');
          awayRosterHeader.style.backgroundColor = color;
        }
        
        this.showNotification('Team updated successfully', 'success');
        this.editingTeamId = null;
      } else {
        // Create new team
        await window.electronAPI.createTeam(this.currentLeague.id, name, color);
        this.showNotification('Team created successfully', 'success');
      }
      
      await this.loadTeams(this.currentLeague.id);
      this.hideModal();
      
      // Reset modal for next use
      document.getElementById('team-modal-title').textContent = 'New Team';
      document.getElementById('save-team-btn').textContent = 'Save Team';
    } catch (error) {
      console.error('Failed to save team:', error);
      this.showNotification('Failed to save team', 'error');
    }
  }

  async savePlayer() {
    const number = document.getElementById('player-jersey').value.trim();
    const name = document.getElementById('player-name').value.trim();
    const description = document.getElementById('player-class').value.trim();

    if (!number || !name) {
      this.showNotification('Please enter jersey number and player name', 'error');
      return;
    }

    try {
      if (this.editingPlayer) {
        // Update existing player
        await window.electronAPI.updatePlayer(this.editingPlayer, number, name, description);
        
        // Reload both rosters to ensure the updated player appears correctly
        if (this.homeTeam) await this.loadPlayers(this.homeTeam.id, true);
        if (this.awayTeam) await this.loadPlayers(this.awayTeam.id, false);
      } else {
        // Create new player - need teamId for this
        const teamId = this.editingTeam;
        if (!teamId) {
          this.showNotification('No team selected', 'error');
          return;
        }
        
        await window.electronAPI.createPlayer(teamId, number, name, description);
        
        // Reload the appropriate roster
        const isHome = teamId === (this.homeTeam ? this.homeTeam.id : null);
        await this.loadPlayers(teamId, isHome);
      }
      
      // Check if 'Add another player' is checked and we're creating a new player
      const addAnother = document.getElementById('add-another-player').checked;
      const isCreating = !this.editingPlayer;
      
      if (addAnother && isCreating) {
        // Reset form but keep modal open
        this.resetPlayerForm();
        document.getElementById('player-jersey').focus();
        this.showNotification('Player created successfully. Add another player.', 'success');
      } else {
        this.hideModal();
        this.showNotification(this.editingPlayer ? 'Player updated successfully' : 'Player created successfully', 'success');
      }
    } catch (error) {
      console.error('Failed to save player:', error);
      this.showNotification('Failed to save player', 'error');
    }
  }

  // ===== EDIT OPERATIONS =====
  editLeague() {
    if (!this.currentLeague) {
      this.showNotification('No league selected', 'error');
      return;
    }

    // Set modal title and button text for editing
    document.getElementById('league-modal-title').textContent = 'Edit League';
    document.getElementById('save-league-btn').textContent = 'Update League';

    // Populate form with current league data
    document.getElementById('league-name').value = this.currentLeague.name;
    document.getElementById('foul-reset-period').value = this.currentLeague.foul_reset_period;
    document.getElementById('bonus-fouls').value = this.currentLeague.bonus_fouls;
    document.getElementById('double-bonus-fouls').value = this.currentLeague.double_bonus_fouls;

    // Set editing flag
    this.editingLeague = this.currentLeague.id;

    this.showModal('new-league-modal');
  }

  editTeam(team) {
    if (!team) {
      this.showNotification('No team selected', 'error');
      return;
    }

    // Set modal title and button text for editing
    document.getElementById('team-modal-title').textContent = 'Edit Team';
    document.getElementById('save-team-btn').textContent = 'Update Team';

    // Populate form with current team data
    document.getElementById('team-name').value = team.name;
    document.getElementById('team-color').value = team.color;

    // Update color preview
    document.getElementById('team-color-preview').style.backgroundColor = team.color;

    // Set editing flag
    this.editingTeamId = team.id;

    this.showModal('new-team-modal');
  }

  // ===== UTILITY METHODS =====
  showAddPlayerModal(isHome) {
    const team = isHome ? this.homeTeam : this.awayTeam;
    if (!team) {
      this.showNotification('Please select a team first', 'error');
      return;
    }

    this.editingTeam = team.id;
    this.editingPlayer = null;
    
    // Reset modal title and button text for new player
    document.getElementById('player-modal-title').textContent = 'New Player';
    document.getElementById('save-player-btn').textContent = 'Save Player';
    
    // Clear form for new player
    document.getElementById('player-jersey').value = '';
    document.getElementById('player-name').value = '';
    document.getElementById('player-class').value = '';
    document.getElementById('add-another-player').checked = false;
    
    // Show 'Add another player' checkbox when adding new player
    const addAnotherGroup = document.getElementById('add-another-player').closest('.form-group');
    addAnotherGroup.style.display = 'block';
    
    this.showModal('new-player-modal');
    // Focus on jersey number field
    setTimeout(() => document.getElementById('player-jersey').focus(), 100);
  }

  resetPlayerForm() {
    // Clear form fields
    document.getElementById('player-jersey').value = '';
    document.getElementById('player-name').value = '';
    document.getElementById('player-class').value = '';
    // Keep the 'Add another player' checkbox state as is
  }

  showNewLeagueModal() {
    // Reset modal title and button text for new league
    document.getElementById('league-modal-title').textContent = 'New League';
    document.getElementById('save-league-btn').textContent = 'Create League';
    
    // Clear editing flag
    this.editingLeague = null;
    
    // Clear form fields
    document.getElementById('league-name').value = '';
    document.getElementById('foul-reset-period').value = 'half';
    document.getElementById('bonus-fouls').value = '7';
    document.getElementById('double-bonus-fouls').value = '10';
    
    this.showModal('new-league-modal');
  }

  showNewTeamModal() {
    console.log('showNewTeamModal called', this.currentLeague);
    
    if (!this.currentLeague) {
      this.showNotification('Please select a league first', 'error');
      return;
    }
    
    console.log('Opening new team modal');
    
    // Reset modal title and button text for new team
    document.getElementById('team-modal-title').textContent = 'New Team';
    document.getElementById('save-team-btn').textContent = 'Save Team';
    
    // Clear editing flag
    this.editingTeamId = null;
    
    // Clear form fields
    document.getElementById('team-name').value = '';
    
    // Reset color picker to default
    const colorInput = document.getElementById('team-color');
    const colorPreview = document.getElementById('team-color-preview');
    
    if (colorInput) {
      colorInput.value = '#000000';
    }
    if (colorPreview) {
      colorPreview.style.backgroundColor = '#000000';
    }
    
    this.showModal('new-team-modal');
  }

  editPlayer(playerId) {
    // Find the player data and populate the form
    let player = null;
    for (const players of [this.currentPlayers.home, this.currentPlayers.away]) {
      player = players.find(p => p.id === parseInt(playerId));
      if (player) break;
    }
    
    if (player) {
      this.editingPlayer = playerId;
      
      // Determine which team this player belongs to
      const isHomeTeamPlayer = this.currentPlayers.home.some(p => p.id === parseInt(playerId));
      this.editingTeam = isHomeTeamPlayer ? 
        (this.homeTeam ? this.homeTeam.id : null) : 
        (this.awayTeam ? this.awayTeam.id : null);
      
      // Update modal title and button text
      document.getElementById('player-modal-title').textContent = 'Edit Player';
      document.getElementById('save-player-btn').textContent = 'Update Player';
      
      // Populate form with existing data and ensure fields are editable
      const jerseyField = document.getElementById('player-jersey');
      const nameField = document.getElementById('player-name');
      const classField = document.getElementById('player-class');
      
      if (jerseyField) {
        jerseyField.readOnly = false;
        jerseyField.disabled = false;
        jerseyField.value = player.jersey_number;
      }
      if (nameField) {
        nameField.readOnly = false;
        nameField.disabled = false;
        nameField.value = player.name;
      }
      if (classField) {
        classField.readOnly = false;
        classField.disabled = false;
        classField.value = player.description || '';
      }
      
      // Hide 'Add another player' checkbox when editing
      const addAnotherGroup = document.getElementById('add-another-player').closest('.form-group');
      addAnotherGroup.style.display = 'none';
      
      this.showModal('new-player-modal');
      // Focus on jersey number field
      setTimeout(() => document.getElementById('player-jersey').focus(), 100);
    }
  }

  async deletePlayer(playerId) {
    // Find the player data to show their name in confirmation
    let player = null;
    for (const players of [this.currentPlayers.home, this.currentPlayers.away]) {
      player = players.find(p => p.id === parseInt(playerId));
      if (player) break;
    }
    
    const playerName = player ? player.name : 'this player';
    if (!confirm(`Are you sure you want to delete ${playerName}? This action cannot be undone.`)) {
      return;
    }

    try {
      await window.electronAPI.deletePlayer(playerId);
      
      // Reload both rosters to reflect changes
      if (this.homeTeam) await this.loadPlayers(this.homeTeam.id, true);
      if (this.awayTeam) await this.loadPlayers(this.awayTeam.id, false);
      
      this.showNotification('Player deleted successfully', 'success');
    } catch (error) {
      console.error('Failed to delete player:', error);
      this.showNotification('Failed to delete player', 'error');
    }
  }

  swapTeams() {
    // Get the DOM elements
    const homeRoster = document.getElementById('home-roster');
    const awayRoster = document.getElementById('away-roster');
    const gameBoard = document.querySelector('.game-board');
    
    // Always swap the visual order of the rosters
    if (!this.teamsSwapped) {
      // First swap: away roster goes to the left (first position)
      gameBoard.insertBefore(awayRoster, homeRoster);
      this.teamsSwapped = true;
      console.log('Teams swapped: Away team now on left');
    } else {
      // Swap back: home roster goes to the left (original position)
      gameBoard.insertBefore(homeRoster, awayRoster);
      this.teamsSwapped = false;
      console.log('Teams swapped back: Home team now on left');
    }
    
    // Always swap the team object references
    const tempTeam = this.homeTeam;
    this.homeTeam = this.awayTeam;
    this.awayTeam = tempTeam;
    
    // Always swap the game statistics
    const tempStats = this.gameStats.home;
    this.gameStats.home = this.gameStats.away;
    this.gameStats.away = tempStats;
    
    console.log('Teams swapped - all data preserved');
  }

  resetGame() {
    // Reset all game statistics
    Object.keys(this.gameStats.home).forEach(playerId => {
      this.gameStats.home[playerId] = {
        fouls: [0, 0, 0, 0],
        fieldGoals: 0,
        threePointers: 0,
        freeThrows: 0,
        isCheckedIn: false
      };
    });

    Object.keys(this.gameStats.away).forEach(playerId => {
      this.gameStats.away[playerId] = {
        fouls: [0, 0, 0, 0],
        fieldGoals: 0,
        threePointers: 0,
        freeThrows: 0,
        isCheckedIn: false
      };
    });

    this.currentPeriod = 1;
    document.getElementById('period-select').value = '1';
    
    // Reload rosters to reflect reset stats
    if (this.homeTeam) this.loadPlayers(this.homeTeam.id, true);
    if (this.awayTeam) this.loadPlayers(this.awayTeam.id, false);
    
    this.showNotification('Game reset successfully', 'success');
  }

  resetFouls() {
    // Note: Individual player fouls never reset during a game
    // Team foul counts automatically reset based on the league's foul_reset_period
    // This function is kept for potential future use but doesn't modify player fouls
    
    this.updateAllFoulDisplays();
    this.showNotification('Team foul counts updated for current period', 'success');
  }

  updatePeriodOptions() {
    const periodSelect = document.getElementById('period-select');
    const league = this.getCurrentLeague();
    
    if (!league) return;

    periodSelect.innerHTML = '';
    
    if (league.foul_reset_period === 'quarter') {
      for (let i = 1; i <= 4; i++) {
        const option = document.createElement('option');
        option.value = i;
        option.textContent = `${i}${this.getOrdinalSuffix(i)} Quarter`;
        periodSelect.appendChild(option);
      }
    } else {
      const option1 = document.createElement('option');
      option1.value = '1';
      option1.textContent = '1st Half';
      periodSelect.appendChild(option1);
      
      const option2 = document.createElement('option');
      option2.value = '2';
      option2.textContent = '2nd Half';
      periodSelect.appendChild(option2);
    }
  }

  getOrdinalSuffix(num) {
    const suffixes = ['th', 'st', 'nd', 'rd'];
    const v = num % 100;
    return suffixes[(v - 20) % 10] || suffixes[v] || suffixes[0];
  }

  updateAllFoulDisplays() {
    document.querySelectorAll('.player-row').forEach(row => {
      this.updatePlayerFoulDisplay(row);
    });
    
    this.updateTeamFoulDisplay(true);
    this.updateTeamFoulDisplay(false);
  }

  clearRosters() {
    document.getElementById('home-players').innerHTML = '';
    document.getElementById('away-players').innerHTML = '';
    
    // Reset team foul displays
    document.getElementById('home-team-fouls').textContent = '0';
    document.getElementById('away-team-fouls').textContent = '0';
    document.getElementById('home-bonus-indicator').textContent = '';
    document.getElementById('away-bonus-indicator').textContent = '';
    document.getElementById('home-bonus-indicator').className = 'bonus-indicator';
    document.getElementById('away-bonus-indicator').className = 'bonus-indicator';
  }

  clearRosterForTeam(isHome) {
    const playersId = isHome ? 'home-players' : 'away-players';
    const teamFoulsId = isHome ? 'home-team-fouls' : 'away-team-fouls';
    const bonusIndicatorId = isHome ? 'home-bonus-indicator' : 'away-bonus-indicator';
    
    document.getElementById(playersId).innerHTML = '';
    document.getElementById(teamFoulsId).textContent = '0';
    document.getElementById(bonusIndicatorId).textContent = '';
    document.getElementById(bonusIndicatorId).className = 'bonus-indicator';
  }

  getCurrentLeague() {
    return this.currentLeague;
  }

  showNotification(message, type = 'info') {
    // Simple notification system - could be enhanced with a proper toast library
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.textContent = message;
    notification.style.cssText = `
      position: fixed;
      top: 20px;
      right: 20px;
      background: ${type === 'error' ? '#ef4444' : type === 'success' ? '#10b981' : '#3b82f6'};
      color: white;
      padding: 12px 24px;
      border-radius: 6px;
      box-shadow: 0 4px 6px -1px rgb(0 0 0 / 0.1);
      z-index: 10000;
      opacity: 0;
      transform: translateX(100%);
      transition: all 0.3s ease;
    `;
    
    document.body.appendChild(notification);
    
    // Show notification
    setTimeout(() => {
      notification.style.opacity = '1';
      notification.style.transform = 'translateX(0)';
    }, 10);
    
    // Hide and remove notification
    setTimeout(() => {
      notification.style.opacity = '0';
      notification.style.transform = 'translateX(100%)';
      setTimeout(() => {
        if (notification.parentNode) {
          notification.parentNode.removeChild(notification);
        }
      }, 300);
    }, 3000);
  }
}

// Initialize the application when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
  console.log('DOM Content Loaded');
  console.log('ElectronAPI available:', !!window.electronAPI);
  
  if (window.electronAPI) {
    console.log('ElectronAPI methods:', Object.keys(window.electronAPI));
  } else {
    console.error('ElectronAPI not available at DOM load');
  }
  
  try {
    window.app = new BasketballRosterManager();
  } catch (error) {
    console.error('Failed to create BasketballRosterManager:', error);
  }
});