export type ReserveComponentType = 'Indemnity' | 'Expense' | 'ALAE' | 'SubrogationRecoverable';
export type ReserveComponentStatus = 'Active' | 'Closed';
export type ReserveTransactionType = 'Add' | 'Adjust' | 'Reverse';
export type ReserveApprovalStatus =
  | 'PendingApproval'
  | 'AutoApproved'
  | 'Approved'
  | 'Rejected'
  | 'Cancelled';
export type GlPostingStatus = 'Pending' | 'Posted' | 'Failed' | 'Cancelled';

export interface ReserveHistoryEntry {
  id: string;
  transactionType: ReserveTransactionType;
  amount: number;
  previousBalance: number;
  newBalance: number;
  changeReason: string;
  approvalStatus: ReserveApprovalStatus;
  approvedByUserId: string | null;
  approvedAt: string | null;
  rejectedByUserId: string | null;
  rejectedAt: string | null;
  rejectionReason: string | null;
  postingStatus: GlPostingStatus;
  changeSequence: number;
  submittedByUserId: string | null;
  createdAt: string;
}

export interface ReserveComponent {
  id: string;
  claimId: string;
  componentType: ReserveComponentType;
  currentAmount: number;
  status: ReserveComponentStatus;
  notes: string | null;
  createdAt: string;
  updatedAt: string | null;
  rowVer: string; // base64
  transactions: ReserveHistoryEntry[];
}

export interface OpenReserveRequest {
  rowVersion: string; // Claim RowVer, base64
  componentType: ReserveComponentType;
  amount: number;
  changeReason: string;
  notes?: string | null;
}

export interface AdjustReserveRequest {
  amount: number;
  changeReason: string;
  rowVersion: string; // ClaimReserveComponent RowVer, base64
}

export interface ApproveReserveRequest {
  transactionId: string;
  rowVersion: string;
}

export interface RejectReserveRequest {
  transactionId: string;
  rejectionReason: string;
  rowVersion: string;
}