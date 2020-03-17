import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-detail-item',
  templateUrl: './detail-item.component.html',
  styleUrls: ['./detail-item.component.css']
})
export class DetailItemComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }
  sliderConfigDetail = { "slidesToShow": 3, "slidesToScroll": 3, "arrows" : true};
  afterChange(e) {
    console.log('afterChange');
  }
}
