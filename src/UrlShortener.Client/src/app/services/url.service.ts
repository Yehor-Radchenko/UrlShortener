import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Url } from '../shared/Url';
import { tap } from 'rxjs/internal/operators/tap';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UrlService {
  httpClient: HttpClient = inject(HttpClient);
  
  private serverLink: string = "https://localhost:7284/";

  getUrls(): Observable<any> {
    return this.httpClient.get(`${this.serverLink}Url`);
  }
  
  getUrl(id: number): Observable<any> {
    return this.httpClient.get(`${this.serverLink}Url/${id}`)
      .pipe(tap(_ => console.log("Url received: ", _)));
  }

  addUrl(fullUrl: string): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('accessToken')}`
    });
    return this.httpClient.post(`${this.serverLink}Url`, { fullUrl }, { headers: headers });
  }

  deleteUrl(id: number): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('accessToken')}`
    });
    return this.httpClient.delete(`${this.serverLink}Url/${id}`, { headers: headers });
  }
}