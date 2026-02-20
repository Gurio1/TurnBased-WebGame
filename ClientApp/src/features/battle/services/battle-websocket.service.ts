import { Injectable } from '@angular/core';

import * as signalR from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';
import { BattleData } from '../contracts/models/battle-data';
import { Reward } from '../../../components/reward-modal/models/reward';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class BattleWebsocketService {
  private hubConnection: signalR.HubConnection;
  private battleSubject = new Subject<BattleData>();
  private actionLogSubject = new Subject<string>();
  private battleReward = new Subject<Reward>();
  private battleLose = new Subject<boolean>();

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.apiUrl + '/hubs/battle', {
        withCredentials: true,
        accessTokenFactory: () => {
          return localStorage.getItem(environment.jwtToken)!;
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
    this.subscribeToBattleLose();
    this.subscribeToBattleErrorMessage();
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

  private subscribeToBattleErrorMessage() {
    this.hubConnection.on('BattleErrorMessage', (data: string) => {
      console.log(data);
    });
  }

  private subscribeToActionLog() {
    this.hubConnection.on('log', (data: string) => {
      console.log(data);
      this.actionLogSubject.next(data);
    });
  }

  useAbility(abilityId: number) {
    this.hubConnection
      .invoke('UseAbility', abilityId)
      .catch((err) => console.error('Error sending ability:', err));
  }

  private subscribeToBattleData() {
    this.hubConnection.on('BattleData', (data: BattleData) => {
      this.battleSubject.next(data);
    });
  }

  private subscribeToBattleReward() {
    this.hubConnection.on('BattleReward', (data: Reward) => {
      this.battleReward.next(data);
    });
  }

  private subscribeToBattleLose() {
    this.hubConnection.on('BattleLose', (data: boolean) => {
      this.battleLose.next(data);
    });
  }

  getBattleData(): Observable<BattleData> {
    return this.battleSubject.asObservable();
  }

  getBattleReward(): Observable<Reward> {
    return this.battleReward.asObservable();
  }

  getBattleLose(): Observable<boolean> {
    return this.battleLose.asObservable();
  }

  getActionLog(): Observable<string> {
    return this.actionLogSubject.asObservable();
  }
}
