import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { Url } from '../../shared/Url';
import { UrlService } from '../../services/url.service'
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  urlsList: any;
  urlService: UrlService = inject(UrlService);
  urlInput: string = '';
  errorMessage: string = '';
  successMessage: string = '';

  constructor(public authService: AuthService) { }

  ngOnInit(): void {
    this.getUrls();
  }

  getUrls() {
    this.urlService.getUrls().subscribe({
      next: urlsList => {
        this.urlsList = urlsList || [];
      },
      error: error => {
        console.error('Error fetching URLs:', error);
        this.urlsList = [];
      }
    });
  }

  addNewUrl() {
    if (this.urlInput) {
      this.errorMessage = '';
      this.successMessage = '';
      this.urlService.addUrl(this.urlInput).subscribe({
        next: (response: any) => {
          console.log('Short URL created:', response.shortUrl);
          this.successMessage = `Short URL created: ${response.shortUrl}`;
          this.getUrls();
          this.urlInput = '';
        },
        error: (error) => {
          console.error('Full error object:', error);
          if (error.status === 400) {
            if (error.error && error.error.errors && error.error.errors.FullUrl) {
              this.errorMessage = error.error.errors.FullUrl[0];
            } else {
              this.errorMessage = 'Invalid URL format. Please enter a valid URL.';
            }
          } else if (error.status === 401) {
            this.errorMessage = 'You are not authorized. Please log in again.';
          } else if (error.status === 409) {
            this.errorMessage = 'This URL already exists in our system.';
          } else {
            this.errorMessage = 'An unexpected error occurred. Please try again later.';
          }
        }
      });
    } else {
      this.errorMessage = 'Please enter a URL';
    }
  }

  deleteUrl(id: number) {
    this.urlService.deleteUrl(id).subscribe({
      next: _ => {
        this.successMessage = 'URL deleted successfully';
        this.getUrls();
      },
      error: error => {
        console.error('Full error object:', error);
        if (error.status === 401 || error.status === 403) {
          this.errorMessage = 'You are not authorized to delete this URL.';
        } else if (error.status === 404) {
          this.errorMessage = 'URL not found.';
        } else {
          this.errorMessage = 'An error occurred while deleting the URL. Please try again.';
        }
      }
    });
  }
}
