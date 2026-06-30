export type PolicyStatus = 'Active' | 'Expired' | 'Cancelled';

export interface Policy {
  id: string;
  policyNumber: string;
  clientName: string;
  effectiveDate: string; // DateOnly -> ISO date string
  expirationDate: string;
  status: PolicyStatus;
  coverageTypes: string[];
}

export interface SearchPoliciesParams {
  searchTerm: string;
  maxResults?: number;
}
