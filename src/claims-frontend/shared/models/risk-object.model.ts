export interface ClaimRiskObject {
  id: string;
  assetType: string;
  assetDescription: string;
  damageDescription: string | null;
  isPrimary: boolean;
  assetReference: string | null;
}

export interface AddRiskObjectRequest {
  rowVersion: string; // Claim RowVer, base64 — додається автоматично в API service, не заповнюється формою
  assetType: string;
  assetDescription: string;
  damageDescription?: string | null;
  isPrimary: boolean;
  assetReference?: string | null;
}