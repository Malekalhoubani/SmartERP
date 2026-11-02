import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SalesFinancialRecordDto, SalesFinancialRecordType, UpsertSalesFinancialRecordDto } from '../../features/sales-accounting/dto/sales-financial-record.dto';
import { CompanyContextService } from './company-context.service';

@Injectable({ providedIn: 'root' })
export class SalesFinancialRecordService {
  private readonly http = inject(HttpClient);
  private readonly companyContext = inject(CompanyContextService);
  private readonly baseUrl = `${environment.salesApiUrl}/records`;
  list(type: SalesFinancialRecordType): Observable<SalesFinancialRecordDto[]> { return this.http.get<SalesFinancialRecordDto[]>(`${this.baseUrl}/${type}`,this.options()); }
  create(type: SalesFinancialRecordType,request: UpsertSalesFinancialRecordDto): Observable<SalesFinancialRecordDto> { return this.http.post<SalesFinancialRecordDto>(`${this.baseUrl}/${type}`,request,this.options()); }
  update(type: SalesFinancialRecordType,id: number,request: UpsertSalesFinancialRecordDto): Observable<SalesFinancialRecordDto> { return this.http.put<SalesFinancialRecordDto>(`${this.baseUrl}/${type}/${id}`,request,this.options()); }
  setActive(type: SalesFinancialRecordType,id: number,active: boolean): Observable<void> { return this.http.patch<void>(`${this.baseUrl}/${type}/${id}/${active ? 'activate' : 'deactivate'}`,{},this.options()); }
  private options(): { headers: HttpHeaders } {
    const companyId = this.companyContext.currentCompanyId();
    if (!companyId) throw new Error('A current company must be selected.');
    return { headers: new HttpHeaders().set('X-Company-ID',String(companyId)) };
  }
}
