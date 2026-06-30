export interface ClaimDocument {
  id: string;
  documentType: string;
  documentName: string;
  contentType: string;
  fileSizeBytes: number;
  uploadedAt: string;
  uploadedByUserId: string | null;
  notes: string | null;
  downloadUrl: string;
}

export interface UploadDocumentRequest {
  rowVersion: string; // Claim RowVer, base64
  documentType: string;
  documentName: string;
  file: File;
}

export const DOCUMENT_TYPES = [
  'PoliceReport',
  'MedicalReport',
  'Invoice',
  'Photo',
  'Contract',
  'Correspondence',
  'Other'
] as const;

export type DocumentType = (typeof DOCUMENT_TYPES)[number];
