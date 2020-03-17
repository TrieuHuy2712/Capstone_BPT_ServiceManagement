import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  // slider 1
  slides = [
    { img: "../../../assets/images/banner-1.jpg" },
    { img: "../../../assets/images/banner-2.jpg" },
    { img: "../../../assets/images/banner-3.jpg" }
  ];
  slideConfig = { "slidesToShow": 1, "slidesToScroll": 1, "arrows": false, "autoplay": true, "autoplaySpeed": 500 };


  // slider 2
  slides2 = [
    { img: "../../../assets/images/introduce-1.jpg", tit1: "Vệ sinh nhà", tit2: "Dọn và vệ sinh theo yêu cầu" },
    { img: "../../../assets/images/introduce-2.jpg", tit1: "Dịch vụ sơn", tit2: "Sơn nhà đón tết nào bà con ơi !" },
    { img: "../../../assets/images/introduce-3.jpg", tit1: "Chuyển nhà", tit2:"Chuyển nhà đón tài lộc nào cả nhà" }
  ];
  slideConfig2 = { "slidesToShow": 3, "slidesToScroll": 1, "arrows": false, "autoplay": false};

  //  location slider

  locationSliders = [
    { img: "../../../assets/images/location_1_.png", description: "DaLat"},
    { img: "../../../assets/images/location_2_.png", description: "HaNoi"},
    { img: "../../../assets/images/location_3_.png", description: "SaiGon"},
    { img: "../../../assets/images/location_4_.png", description: "VungTau"},
    { img: "../../../assets/images/location_5_.jpg", description: "NhaTrang"},
    { img: "../../../assets/images/location_6_.png", description: "QuangNinh"},
    { img: "../../../assets/images/location_7_.png", description: "DaNang"},
    { img: "../../../assets/images/location_8_.png", description: "HoiAn"}
  ];
  slideConfig3 = { "slidesToShow": 4, "slidesToScroll": 1, "arrows" : false};

  afterChange(e) {
    console.log('afterChange');
  }
}
