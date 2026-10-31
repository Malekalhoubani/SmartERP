import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  CompanyResponseDto,
  CreateCompanyRequestDto,
  UpdateCompanyRequestDto,
} from '../../features/organization/dto/company/company.dto';

@Injectable({ providedIn: 'root' })
export class OrganizationService {
  private readonly httpClient = inject(HttpClient);
  private readonly companiesUrl = `${environment.organizationApiUrl}/companies`;

  getCompanies(): Observable<CompanyResponseDto[]> {
    return this.httpClient.get<CompanyResponseDto[]>(this.companiesUrl);
  }

  getCompany(companyId: number): Observable<CompanyResponseDto> {
    return this.httpClient.get<CompanyResponseDto>(`${this.companiesUrl}/${companyId}`);
  }

  createCompany(requestDto: CreateCompanyRequestDto): Observable<CompanyResponseDto> {
    return this.httpClient.post<CompanyResponseDto>(this.companiesUrl, requestDto);
  }

  updateCompany(
    companyId: number,
    requestDto: UpdateCompanyRequestDto,
  ): Observable<CompanyResponseDto> {
    return this.httpClient.put<CompanyResponseDto>(
      `${this.companiesUrl}/${companyId}`,
      requestDto,
    );
  }

  activateCompany(companyId: number): Observable<void> {
    return this.httpClient.patch<void>(
      `${this.companiesUrl}/${companyId}/activate`,
      {},
    );
  }

  deactivateCompany(companyId: number): Observable<void> {
    return this.httpClient.patch<void>(
      `${this.companiesUrl}/${companyId}/deactivate`,
      {},
    );
  }
}
