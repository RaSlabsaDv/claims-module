export interface ClaimParty {
  id: string;
  partyRole: string;
  partyType: string;
  firstName: string | null;
  lastName: string | null;
  companyName: string | null;
  email: string | null;
  phone: string | null;
  notes: string | null;
  isActive: boolean;
}

export interface AddPartyRequest {
  rowVersion: string; // Claim RowVer, base64 — додається автоматично в API service, не заповнюється формою
  partyRole: string;
  partyType: string;
  firstName?: string | null;
  lastName?: string | null;
  companyName?: string | null;
  email?: string | null;
  phone?: string | null;
  notes?: string | null;
}