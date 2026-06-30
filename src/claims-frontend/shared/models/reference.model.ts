import { ClaimStatus } from './claim.model';

export interface CauseOfLossCode {
  id: string;
  code: string;
  name: string;
  perilCategory: string;
  sortOrder: number;
}

export interface ClaimStatusInfo {
  status: ClaimStatus;
  validTransitions: ClaimStatus[];
}
