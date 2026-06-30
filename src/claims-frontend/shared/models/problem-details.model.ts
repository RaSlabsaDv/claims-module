export interface ProblemDetails {
  type?: string;
  title: string;
  status: number;
  detail?: string;
  correlationId?: string;
  errors?: Record<string, string[]>;
}
