export interface LossEvent {
  id: string;
  lossDate: string;
  lossDescription: string;
  lossLocation: string | null;
  causeOfLossCode: string;
  estimatedLossAmount: number | null;
  reportDate: string;
  policeReportNumber: string | null;
}