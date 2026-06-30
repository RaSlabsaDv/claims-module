import { LossEvent } from "./loss-event.model";
import { ClaimParty } from "./party.model";
import { ClaimRiskObject } from "./risk-object.model";

export type ClaimStatus =
  | 'Draft'
  | 'Open'
  | 'UnderInvestigation'
  | 'PendingPayment'
  | 'Closed'
  | 'Reopened'
  | 'Withdrawn';

export type ClaimSeverity = 'Minor' | 'Standard' | 'Critical' | 'Catastrophic';

export interface ClaimListItem {
  id: string;
  claimNumber: string;
  policyNumber: string | null;
  clientName: string | null;
  status: ClaimStatus;
  severity: ClaimSeverity;
  reportedDate: string;
  causeOfLossCode: string | null;
  assignedHandlerId: string | null;
  updatedAt: string | null;
}

export interface ListClaimsResult {
  items: ClaimListItem[];
  totalCount: number;
}

export interface ClaimDetail {
  id: string;
  claimNumber: string;
  organisationId: string;
  policyId: string | null;
  policyNumber: string | null;
  clientName: string | null;
  status: ClaimStatus;
  severity: ClaimSeverity;
  reportedDate: string;
  closedAt: string | null;
  createdAt: string;
  updatedAt: string | null;
  assignedHandlerId: string | null;
  closureReason: string | null;
  notes: string | null;
  rowVer: string; // base64
  lossEvent: LossEvent | null;
  parties: ClaimParty[];
  riskObjects: ClaimRiskObject[];
}

export interface CreateClaimRequest {
  policyId?: string | null;
  policyNumber?: string | null;
  clientName?: string | null;
  severity: ClaimSeverity;
  assignedHandlerId?: string | null;
  notes?: string | null;
  lossDate: string;
  lossDescription: string;
  lossLocation?: string | null;
  causeOfLossCode: string;
  estimatedLossAmount?: number | null;
  policeReportNumber?: string | null;
}

export interface CreateClaimResult {
  claimId: string;
  claimNumber: string;
}

export interface ClaimListFilter {
  status?: ClaimStatus | null;
  dateFrom?: string | null;
  dateTo?: string | null;
  assignedHandlerId?: string | null;
  causeOfLossCode?: string | null;
  policyId?: string | null;
  search?: string | null;
  page?: number;
  pageSize?: number;
}