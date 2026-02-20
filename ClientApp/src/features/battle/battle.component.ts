import { CommonModule } from '@angular/common';
import {
  AfterViewChecked,
  Component,
  DestroyRef,
  ElementRef,
  OnInit,
  ViewChild,
  inject,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Ability } from '../../core/models/ability';
import { DefeatModalComponent } from '../../components/defeat-modal/defeat-modal.component';
import { RewardModalComponent } from '../../components/reward-modal/reward-modal.component';
import { Reward } from '../../components/reward-modal/models/reward';
import { BattleData } from './contracts/models/battle-data';
import { BattleWebsocketService } from './services/battle-websocket.service';

@Component({
  selector: 'app-battle',
  standalone: true,
  imports: [CommonModule, RewardModalComponent, DefeatModalComponent],
  templateUrl: './battle.component.html',
  styleUrl: './battle.component.scss',
})
export class BattleComponent implements OnInit, AfterViewChecked {
  private readonly destroyRef = inject(DestroyRef);

  battleData?: BattleData;
  reward?: Reward;
  logs: string[] = [];

  @ViewChild('logContainer') logContainer: ElementRef | undefined;
  defeat = false;

  constructor(private readonly battleService: BattleWebsocketService) {}

  ngOnInit(): void {
    this.battleService
      .getBattleData()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((data) => {
        this.battleData = data;
      });

    this.battleService
      .getActionLog()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((message) => {
        this.logs.push(message);
      });

    this.battleService
      .getBattleReward()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((reward) => {
        this.reward = reward;
      });

    this.battleService
      .getBattleLose()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((defeat) => {
        this.defeat = defeat;
      });
  }

  ngAfterViewChecked(): void {
    this.scrollLogsToBottom();
  }

  useAbility(ability: Ability): void {
    if (ability.currentCooldown === 0) {
      this.battleService.useAbility(ability.id);
      return;
    }

    alert(`${ability.name} is on cooldown!`);
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
    if (!this.logContainer) {
      return;
    }

    this.logContainer.nativeElement.scrollTop = this.logContainer.nativeElement.scrollHeight;
  }
}
