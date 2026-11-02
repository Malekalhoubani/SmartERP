import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { OperationalRecordDto, OperationalRecordType, UpsertOperationalRecordDto } from '../../features/operations/dto/operational-record.dto';
import { CompanyContextService } from './company-context.service';

@Injectable({ providedIn: 'root' })
export class OperationalRecordService {
  private readonly http = inject(HttpClient);
  private readonly companyContext = inject(CompanyContextService);
  private readonly baseUrl = `${environment.inventoryApiUrl}/operations`;

  list(type: OperationalRecordType): Observable<OperationalRecordDto[]> {
    return this.http.get<OperationalRecordDto[]>(`${this.baseUrl}/${type}`,this.options());
  }

  create(type: OperationalRecordType,request: UpsertOperationalRecordDto): Observable<OperationalRecordDto> {
    return this.http.post<OperationalRecordDto>(`${this.baseUrl}/${type}`,request,this.options());
  }

  update(type: OperationalRecordType,recordId: number,request: UpsertOperationalRecordDto): Observable<OperationalRecordDto> {
    return this.http.put<OperationalRecordDto>(`${this.baseUrl}/${type}/${recordId}`,request,this.options());
  }

  setActive(type: OperationalRecordType,recordId: number,isActive: boolean): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${type}/${recordId}/${isActive ? 'activate' : 'deactivate'}`,{},this.options());
  }

  private options(): { headers: HttpHeaders } {
    const companyId = this.companyContext.currentCompanyId();
    if (!companyId) throw new Error('A current company must be selected.');
    return { headers: new HttpHeaders().set('X-Company-ID',String(companyId)) };
  }
}
