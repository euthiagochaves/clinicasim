export interface QuestionAdminItem {
  id: string;
  text: string;
  section: string;
  category: string;
  tags: string | null;
  active: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface FindingAdminItem {
  id: string;
  name: string;
  system: string;
  tags: string | null;
  active: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface QuestionFilters {
  active?: boolean;
  section?: string;
  category?: string;
  search?: string;
}

export interface FindingFilters {
  active?: boolean;
  system?: string;
  search?: string;
}

export interface CreateQuestionPayload {
  text: string;
  section: string;
  category: string;
  tags?: string | null;
}

export interface UpdateQuestionPayload extends CreateQuestionPayload {
  active?: boolean | null;
}

export interface CreateFindingPayload {
  name: string;
  system: string;
  tags?: string | null;
}

export interface UpdateFindingPayload extends CreateFindingPayload {
  active?: boolean | null;
}


export interface AdminCaseItem {
  id: string;
  fullName: string;
  age: number;
  sex: string;
  chiefComplaint: string;
  triage: string;
  active: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CaseFilters {
  active?: boolean;
  search?: string;
  triage?: string;
}

export interface CreateCasePayload {
  fullName: string;
  age: number;
  sex: string;
  chiefComplaint: string;
  triage: string;
}

export interface UpdateCasePayload extends CreateCasePayload {
  active?: boolean;
}

export interface CaseAnswerMapping {
  id: string;
  caseId: string;
  questionId: string;
  questionText: string;
  section: string;
  category: string;
  answerText: string;
}

export interface CreateCaseAnswerPayload {
  questionId: string;
  answerText: string;
}

export interface UpdateCaseAnswerPayload extends CreateCaseAnswerPayload {}

export interface CaseFindingMapping {
  id: string;
  caseId: string;
  findingId: string;
  findingName: string;
  system: string;
  present: boolean;
  detailText: string | null;
}

export interface CreateCaseFindingPayload {
  findingId: string;
  present: boolean;
  detailText?: string | null;
}

export interface UpdateCaseFindingPayload extends CreateCaseFindingPayload {}
