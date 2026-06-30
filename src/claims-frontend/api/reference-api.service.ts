import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CauseOfLossCode, ClaimStatusInfo } from '../shared/models/reference.model';

@Injectable({ providedIn: 'root' })
export class ReferenceApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/reference`;

  getCauseOfLossCodes(): Observable<CauseOfLossCode[]> {
    return this.http.get<CauseOfLossCode[]>(`${this.baseUrl}/cause-of-loss-codes`);
  }

  getClaimStatuses(): Observable<ClaimStatusInfo[]> {
    return this.http.get<ClaimStatusInfo[]>(`${this.baseUrl}/claim-statuses`);
  }
}
