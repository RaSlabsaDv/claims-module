import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../src/environments/environment';
import {
  ClaimDetail,
  ClaimListFilter,
  CreateClaimRequest,
  CreateClaimResult,
  ListClaimsResult
} from '../shared/models/claim.model';
import { AuditLogResult } from '../shared/models/audit-log.model';
import { ClaimDocument } from '../shared/models/document.model';
import { AddRiskObjectRequest } from '../shared/models/risk-object.model';
import { AddPartyRequest } from '../shared/models/party.model';

@Injectable({ providedIn: 'root' })
export class ClaimsApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/claims`;

  listClaims(filter: ClaimListFilter): Observable<ListClaimsResult> {
    let params = new HttpParams();

    if (filter.status) params = params.set('status', filter.status);
    if (filter.dateFrom) params = params.set('dateFrom', filter.dateFrom);
    if (filter.dateTo) params = params.set('dateTo', filter.dateTo);
    if (filter.assignedHandlerId) params = params.set('assignedHandlerId', filter.assignedHandlerId);
    if (filter.causeOfLossCode) params = params.set('causeOfLossCode', filter.causeOfLossCode);
    if (filter.policyId) params = params.set('policyId', filter.policyId);
    if (filter.search) params = params.set('search', filter.search);
    params = params.set('page', filter.page ?? 1);
    params = params.set('pageSize', filter.pageSize ?? 20);

    return this.http.get<ListClaimsResult>(this.baseUrl, { params });
  }

  getClaimDetail(id: string): Observable<ClaimDetail> {
    return this.http.get<ClaimDetail>(`${this.baseUrl}/${id}`);
  }

  getClaimDocuments(id: string): Observable<ClaimDocument[]> {
    return this.http.get<ClaimDocument[]>(`${this.baseUrl}/${id}/documents`);
  }

  getClaimAuditLog(id: string, page = 1, pageSize = 20): Observable<AuditLogResult> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<AuditLogResult>(`${this.baseUrl}/${id}/audit`, { params });
  }

  createClaim(request: CreateClaimRequest): Observable<CreateClaimResult> {
    return this.http.post<CreateClaimResult>(this.baseUrl, request);
  }

  transitionStatus(
    id: string,
    rowVersion: string,
    targetStatus: string,
    reason: string | null
  ): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/status`, {
      rowVersion,
      targetStatus,
      reason
    });
  }

  assignHandler(id: string, handlerId: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/handler`, { handlerId });
  }

  updateNotes(id: string, notes: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/notes`, { notes });
  }

  addParty(id: string, rowVersion: string, party: Record<string, AddPartyRequest>): Observable<{ partyId: string }> {
    return this.http.post<{ partyId: string }>(`${this.baseUrl}/${id}/parties`, {
      rowVersion,
      ...party
    });
  }

  removeParty(id: string, partyId: string, rowVersion: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}/parties/${partyId}`, {
      body: { rowVersion }
    });
  }

  addRiskObject(
    id: string,
    rowVersion: string,
    riskObject: Record<string, AddRiskObjectRequest>
  ): Observable<{ riskObjectId: string }> {
    return this.http.post<{ riskObjectId: string }>(`${this.baseUrl}/${id}/risk-objects`, {
      rowVersion,
      ...riskObject
    });
  }

  uploadDocument(
    id: string,
    rowVersion: string,
    documentType: string,
    documentName: string,
    file: File
  ): Observable<{ documentId: string }> {
    const formData = new FormData();
    formData.append('rowVersion', rowVersion);
    formData.append('documentType', documentType);
    formData.append('documentName', documentName);
    formData.append('file', file);

    return this.http.post<{ documentId: string }>(`${this.baseUrl}/${id}/documents`, formData);
  }
}
