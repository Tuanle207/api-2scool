import { Component, OnInit } from '@angular/core';
import { RealTimeService } from '../services/real-time.service';

@Component({
  selector: 'lib-real-time',
  template: ` <p>real-time works!</p> `,
  styles: [],
})
export class RealTimeComponent implements OnInit {
  constructor(private service: RealTimeService) {}

  ngOnInit(): void {
    this.service.sample().subscribe(console.log);
  }
}
