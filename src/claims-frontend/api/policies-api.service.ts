import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Policy, SearchPoliciesParams } from '../shared/models/policy.model';

@Injectable({ providedIn: 'root' })
export class PoliciesApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/policies`;

  search(params: SearchPoliciesParams): Observable<Policy[]> {
    let httpParams = new HttpParams().set('searchTerm', params.searchTerm);

    if (params.maxResults) {
      httpParams = httpParams.set('maxResults', params.maxResults);
    }

    return this.http.get<Policy[]>(`${this.baseUrl}/search`, { params: httpParams });
  }

  getById(id: string): Observable<Policy> {
    return this.http.get<Policy>(`${this.baseUrl}/${id}`);
  }
}
