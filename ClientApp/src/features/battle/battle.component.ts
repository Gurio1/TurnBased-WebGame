import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { Ability } from '../../core/models/ability';
import { CommonModule } from '@angular/common';
import { BattleResponse } from './Contracts/Responses/WebSocket/battle-response';
import { BattleWebsocketService } from './services/battle-websocket.service';
import { BattleData } from './Contracts/models/battle-data';
import { MatDialog } from '@angular/material/dialog';
import { RewardModalComponent } from '../../components/reward-modal/reward-modal.component';

@Component({
  selector: 'app-battle',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './battle.component.html',
  styleUrl: './battle.component.scss',
})
export class BattleComponent implements OnInit {
  battleData?: BattleData;
  logs: string[] = [];

  @ViewChild('logContainer') logContainer!: ElementRef;

  constructor(
    private battleService: BattleWebsocketService,
    private dialog: MatDialog
  ) {}

  ngOnInit() {
    this.battleService.getBattleData().subscribe((data) => {
      this.battleData = data;
    });

    this.battleService.getActionLog().subscribe((data) => {
      console.log(data.message);
      this.logs.push(data.message);
      console.log(this.logs);
    });

    this.battleService.getBattleReward().subscribe((reward) => {
      this.dialog.open(RewardModalComponent, {
        width: '400px',
        data: reward,
      });
    });
  }

  useAbility(ability: Ability) {
    if (ability.currentCooldown === 0) {
      this.battleService.useAbility(ability.id);
    } else {
      alert(`${ability.name} is on cooldown!`);
    }
  }
}
