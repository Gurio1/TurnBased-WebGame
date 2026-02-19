import {
  AfterViewChecked,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
} from '@angular/core';
import { Ability } from '../../core/models/ability';
import { CommonModule } from '@angular/common';
import { BattleResponse } from './Contracts/Responses/WebSocket/battle-response';
import { BattleWebsocketService } from './services/battle-websocket.service';
import { BattleData } from './Contracts/models/battle-data';
import { RewardModalComponent } from '../../components/reward-modal/reward-modal.component';
import { Router } from '@angular/router';
import { Reward } from '../../components/reward-modal/models/reward';
import { DefeatModalComponent } from '../../components/defeat-modal/defeat-modal.component';

@Component({
  selector: 'app-battle',
  standalone: true,
  imports: [CommonModule, RewardModalComponent, DefeatModalComponent],
  templateUrl: './battle.component.html',
  styleUrl: './battle.component.scss',
})
export class BattleComponent implements OnInit, AfterViewChecked {
  battleData?: BattleData;
  reward?: Reward;
  logs: string[] = [];

  @ViewChild('logContainer') logContainer: ElementRef | undefined;
  defeat: boolean = false;

  constructor(
    private battleService: BattleWebsocketService,
    private router: Router
  ) {}

  ngOnInit() {
    this.battleService.getBattleData().subscribe((data) => {
      this.battleData = data;
    });

    this.battleService.getActionLog().subscribe((message) => {
      this.logs.push(message);
    });

    this.battleService.getBattleReward().subscribe((reward) => {
      console.log('Reward:', reward);
      this.reward = reward;
    });
    this.battleService.getBattleLose().subscribe((defeat) => {
      console.log('Defeat:', defeat);
      this.defeat = defeat;
    });
  }

  ngAfterViewChecked() {
    this.scrollLogsToBottom();
  }

  useAbility(ability: Ability) {
    if (ability.currentCooldown === 0) {
      this.battleService.useAbility(ability.id);
    } else {
      alert(`${ability.name} is on cooldown!`);
    }
  }

  get playerHealthPercentage(): number {
    if (!this.battleData?.combatPlayer?.stats.maxHealth) return 0;
    const { currentHealth, maxHealth } = this.battleData.combatPlayer.stats;
    return (currentHealth / maxHealth) * 100;
  }

  get monsterHealthPercentage(): number {
    if (!this.battleData?.monster?.stats.maxHealth) return 0;

    const { currentHealth, maxHealth } = this.battleData.monster.stats;
    return (currentHealth / maxHealth) * 100;
  }

  getLogClass(log: string): string {
    if (log.includes('heal')) return 'log-heal';
    if (log.includes('critical')) return 'critical';
    if (log.includes('miss')) return 'miss';
    return '';
  }

  private scrollLogsToBottom(): void {
    try {
      if (this.logContainer) {
        this.logContainer.nativeElement.scrollTop =
          this.logContainer.nativeElement.scrollHeight;
      }
    } catch (err) {
      console.error('Failed to scroll logs:', err);
    }
  }
}
