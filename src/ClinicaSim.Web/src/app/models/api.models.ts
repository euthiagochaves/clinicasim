export interface CaseListItemDto {
  caseId: string;
  fullName: string;
  age: number;
  sex: string;
  chiefComplaint: string;
  triage: string;
}

export interface StartSessionRequest { caseId: string; }

export interface StartSessionResponse {
  sessionCode: string;
  status: string;
  startedAt: string;
  case: CaseListItemDto;
  sections: SessionSectionDto[];
}

export interface SessionSectionDto {
  sectionId: string;
  name: string;
  categories: SessionCategoryDto[];
}

export interface SessionCategoryDto {
  categoryId: string;
  name: string;
  questions: SessionQuestionDto[];
}

export interface SessionQuestionDto {
  questionId: string;
  text: string;
}

export interface PostEventRequest { questionId: string; }

export interface PostEventResponse {
  eventId: string;
  occurredAt: string;
  sectionName: string;
  categoryName: string;
  questionText: string;
  answerText: string;
}

export interface SessionInfoResponse {
  sessionCode: string;
  status: 'Active' | 'Finalized';
  startedAt: string;
  finishedAt: string | null;
  case: CaseListItemDto;
}

export interface ClinicalNoteRequest {
  summaryText: string;
  probableDiagnosisText: string;
  conductStudiesText: string;
  conductTreatmentText: string;
}

export interface ClinicalNoteResponse {
  summaryText: string | null;
  probableDiagnosisText: string | null;
  conductStudiesText: string | null;
  conductTreatmentText: string | null;
}

export interface DifferentialItemDto {
  rank: number;
  text: string;
}

export interface SaveDifferentialsRequest {
  items: DifferentialItemDto[];
}

export interface FinalizeResponse {
  sessionCode: string;
  status: 'Finalized';
  finishedAt: string;
}
