import { Injectable } from '@angular/core';

import * as signalR from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';
import { ActionLogResponse } from '../Contracts/Responses/WebSocket/action-log-response';
import { BattleResponse } from '../Contracts/Responses/WebSocket/battle-response';

import { API_URL, JWT_TOKEN } from '../../../constants';
import { BattleData } from '../Contracts/models/battle-data';
import { Reward } from '../../../components/reward-modal/models/reward';

@Injectable({
  providedIn: 'root',
})
export class BattleWebsocketService {
  private hubConnection: signalR.HubConnection;
  private battleSubject = new Subject<BattleData>();
  private actionLogSubject = new Subject<ActionLogResponse>();
  private battleReward = new Subject<Reward>();

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(API_URL + 'hubs/battle', {
        withCredentials: true,
        accessTokenFactory: () => {
          return localStorage.getItem(JWT_TOKEN)!;
        },
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect()
      .build();

    this.startConnection();
    this.subscribeToActionLog();
    this.subscribeToBattleData();
    this.subscribeToBattleReward();
  }

  private startConnection() {
    this.hubConnection
      .start()
      .then(() => console.log('Connected to SignalR hub'))
      .catch((err) =>
        //TODO : Catch 401 error code
        console.error('Error connecting to SignalR hub:', err)
      );
  }
  private subscribeToActionLog() {
    this.hubConnection.on('Log', (data: ActionLogResponse) => {
      this.actionLogSubject.next(data);
    });
  }

  useAbility(abilityId: number) {
    this.hubConnection
      .invoke('UseAbility', abilityId)
      .catch((err) => console.error('Error sending ability:', err));
  }

  private subscribeToBattleData() {
    this.hubConnection.on('ReceiveBattleData', (data: BattleData) => {
      console.log(data);
      this.battleSubject.next(data);
    });
  }

  private subscribeToBattleReward() {
    this.hubConnection.on('ReceiveBattleReward', (data: Reward) => {
      console.log(data);
      this.battleReward.next(data);
    });
  }

  getBattleData(): Observable<BattleData> {
    return this.battleSubject.asObservable();
  }

  getBattleReward(): Observable<Reward> {
    return this.battleReward.asObservable();
  }

  getActionLog(): Observable<ActionLogResponse> {
    return this.actionLogSubject.asObservable();
  }
}
