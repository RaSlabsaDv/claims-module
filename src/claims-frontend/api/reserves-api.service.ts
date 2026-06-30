import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  AdjustReserveRequest,
  ApproveReserveRequest,
  OpenReserveRequest,
  RejectReserveRequest,
  ReserveComponent
} from '../shared/models/reserve.model';

@Injectable({ providedIn: 'root' })
export class ReservesApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  listReserves(claimId: string): Observable<ReserveComponent[]> {
    return this.http.get<ReserveComponent[]>(`${this.baseUrl}/claims/${claimId}/reserves`);
  }

  openReserve(claimId: string, request: OpenReserveRequest): Observable<{ reserveId: string }> {
    return this.http.post<{ reserveId: string }>(`${this.baseUrl}/claims/${claimId}/reserves`, request);
  }

  adjustReserve(
    claimId: string,
    reserveId: string,
    request: AdjustReserveRequest
  ): Observable<void> {
    return this.http.put<void>(
      `${this.baseUrl}/claims/${claimId}/reserves/${reserveId}`,
      request
    );
  }

  approveReserve(claimId: string, reserveId: string, request: ApproveReserveRequest): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrl}/claims/${claimId}/reserves/${reserveId}/approve`,
      request
    );
  }

  rejectReserve(claimId: string, reserveId: string, request: RejectReserveRequest): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrl}/claims/${claimId}/reserves/${reserveId}/reject`,
      request
    );
  }

  overrideAggregateLimit(claimId: string, reserveId: string): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrl}/claims/${claimId}/reserves/${reserveId}/override-limit`,
      {}
    );
  }
}
