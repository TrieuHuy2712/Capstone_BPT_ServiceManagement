import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-list-of-item',
  templateUrl: './list-of-item.component.html',
  styleUrls: ['./list-of-item.component.css']
})
export class ListOfItemComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  sliderConfigPopular = { "slidesToShow": 4, "slidesToScroll": 1, "arrows" : true};
  afterChange(e) {
    console.log('afterChange');
  }

}
