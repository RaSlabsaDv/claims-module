export interface AuditLogEntry {
  id: string;
  eventType: string;
  description: string;
  oldValue: string | null; // JSON string
  newValue: string | null; // JSON string
  relatedEntityId: string | null;
  relatedEntityType: string | null;
  correlationId: string | null;
  createdAt: string;
  createdByUserId: string | null;
}

export interface AuditLogResult {
  items: AuditLogEntry[];
  totalCount: number;
}
