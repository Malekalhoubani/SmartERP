import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { CatalogItemDto, CatalogType, UpsertCatalogItemDto } from '../../features/catalog/dto/catalog.dto';
import { CompanyContextService } from './company-context.service';

@Injectable({ providedIn: 'root' })
export class CatalogService {
  private readonly http = inject(HttpClient);
  private readonly companyContext = inject(CompanyContextService);
  private readonly baseUrl = `${environment.inventoryApiUrl}/catalog`;

  list(type: CatalogType): Observable<CatalogItemDto[]> {
    return this.http.get<CatalogItemDto[]>(`${this.baseUrl}/${type}`,this.options());
  }

  create(type: CatalogType,request: UpsertCatalogItemDto): Observable<CatalogItemDto> {
    return this.http.post<CatalogItemDto>(`${this.baseUrl}/${type}`,request,this.options());
  }

  update(type: CatalogType,itemId: number,request: UpsertCatalogItemDto): Observable<CatalogItemDto> {
    return this.http.put<CatalogItemDto>(`${this.baseUrl}/${type}/${itemId}`,request,this.options());
  }

  setStatus(type: CatalogType,itemId: number,isActive: boolean): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${type}/${itemId}/${isActive ? 'activate' : 'deactivate'}`,{},this.options());
  }

  private options(): { headers: HttpHeaders } {
    const companyId = this.companyContext.currentCompanyId();
    if (!companyId) throw new Error('A current company must be selected.');
    return { headers: new HttpHeaders().set('X-Company-ID',String(companyId)) };
  }
}
