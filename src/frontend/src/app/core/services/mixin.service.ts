import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface MixinEntity {
  id: string;
  name: string;
  displayName: string;
  description: string;
  schema: any;
  createdAt: Date;
  updatedAt: Date;
}

@Injectable({
  providedIn: 'root'
})
export class MixinService {

  constructor(private http: HttpClient) { }

  getAll(): Observable<MixinEntity[]> {
    return this.http.get<MixinEntity[]>('/api/mixins');
  }

  getById(id: string): Observable<MixinEntity> {
    return this.http.get<MixinEntity>(`/api/mixins/${id}`);
  }

  create(mixin: Omit<MixinEntity, 'id' | 'createdAt' | 'updatedAt'>): Observable<MixinEntity> {
    return this.http.post<MixinEntity>('/api/mixins', mixin);
  }

  update(id: string, mixin: Omit<MixinEntity, 'id' | 'createdAt' | 'updatedAt'>): Observable<MixinEntity> {
    return this.http.put<MixinEntity>(`/api/mixins/${id}`, mixin);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`/api/mixins/${id}`);
  }

  validateSchema(schema: any): Observable<any> {
    return this.http.post('/api/mixins/validate-schema', schema);
  }
}