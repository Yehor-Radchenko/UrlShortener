import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UrlService } from '../../services/url.service';
import { Url } from '../../shared/Url';

@Component({
  selector: 'app-url-detail',
  standalone: true,
  imports: [
    CommonModule,
  ],
  templateUrl: './url-details.component.html',
  styleUrl: './url-details.component.css'
})
export class UrlDetailsComponent implements OnInit {
  urlService = inject(UrlService);
  route: ActivatedRoute = inject(ActivatedRoute);
  url: Url | null = null;

  ngOnInit() {
    const urlId = Number(this.route.snapshot.params['id']);
    this.urlService.getUrl(urlId).subscribe(url => {
      this.url = url;
    });
  }
}