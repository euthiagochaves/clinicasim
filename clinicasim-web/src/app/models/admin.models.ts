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
