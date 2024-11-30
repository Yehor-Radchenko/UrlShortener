import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({
  name: 'defaultDate',
  standalone: true
})
export class DefaultDatePipe implements PipeTransform {
  transform(value: any, format: string = 'dd MMM yyyy'): string {
    if (value) {
      const datePipe = new DatePipe('en-US');
      return datePipe.transform(value, format) || 'N/A';
    }
    return 'N/A';
  }
}