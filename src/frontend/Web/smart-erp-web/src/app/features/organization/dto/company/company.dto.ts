export interface CreateCompanyRequestDto {
  name: string;
  legalName: string;
  code: string;
  registrationNumber: string | null;
  taxNumber: string | null;
  email: string | null;
  phone: string | null;
}

export interface UpdateCompanyRequestDto {
  name: string;
  legalName: string;
  registrationNumber: string | null;
  taxNumber: string | null;
  email: string | null;
  phone: string | null;
}

export interface CompanyResponseDto {
  id: number;
  name: string;
  legalName: string;
  code: string;
  registrationNumber: string | null;
  taxNumber: string | null;
  email: string | null;
  phone: string | null;
  isActive: boolean;
  createdAtUtc: string;
  updatedAtUtc: string | null;
}
