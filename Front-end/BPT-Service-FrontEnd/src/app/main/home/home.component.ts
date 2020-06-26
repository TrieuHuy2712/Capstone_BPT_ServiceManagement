import { Component, OnInit, ViewChild } from '@angular/core';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { ModalDirective } from 'ngx-bootstrap';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  locations: any[];
  public category: any[];
  public news: any[];
  public hotService: any[];
  @ViewChild("modalEditProfile", { static: false })
  public modalEditProfile: ModalDirective;
  public detailNews: any[];
  public subDetailNews: any[];

  // detail of new params
  public detailTitle: string;
  public detailAuthor: string;
  public detailContent: string;
  public detailImage: string;
  public detailProviderName: string;

  constructor(
    private _dataService: DataService,
    private _notificationService: NotificationService,
  ) { }

  ngOnInit(

  ) {
    this.loadDataOfLocation();
    this.getAllCategory();
    this.getAllNews();
    this.getRecommendService();
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
    { img: "../../../assets/images/introduce-3.jpg", tit1: "Chuyển nhà", tit2: "Chuyển nhà đón tài lộc nào cả nhà" }
  ];
  slideConfig2 = { "slidesToShow": 5, "slidesToScroll": 1, "arrows": true, "autoplay": false };

  //  location slider

  locationSliders = [
    { img: "../../../assets/images/location_1_.png", description: "DaLat" },
    { img: "../../../assets/images/location_2_.png", description: "HaNoi" },
    { img: "../../../assets/images/location_3_.png", description: "SaiGon" },
    { img: "../../../assets/images/location_4_.png", description: "VungTau" },
    { img: "../../../assets/images/location_5_.jpg", description: "NhaTrang" },
    { img: "../../../assets/images/location_6_.png", description: "QuangNinh" },
    { img: "../../../assets/images/location_7_.png", description: "DaNang" },
    { img: "../../../assets/images/location_8_.png", description: "HoiAn" }
  ];
  slideConfig3 = { "slidesToShow": 4, "slidesToScroll": 1, "arrows": false, "autoplay": false };

  // slider config of service
  sliderConfigOfService = {"slidesToShow": 4, "slidesToScroll": 1, "arrows": false, "autoplay": false};

  afterChange(e) {
    console.log('afterChange');
  }

  // show detail of news of provider
  showAddModal(id: any) {    
    this.modalEditProfile.show();
    this._dataService.getNoAu("/ProviderNews/GetAllPagingProviderNews?isAdminPage=true&filter=1")
    .subscribe((response: any) =>{
      this.detailNews = response.results;
      this.detailTitle = this.detailNews.find(x => x.id == id).title;
      this.detailAuthor = this.detailNews.find(x => x.id == id).author;
      this.detailContent = this.detailNews.find(x => x.id == id).content;
      this.detailImage = this.detailNews.find(x => x.id == id).imgPath;
      this.detailProviderName = this.detailNews.find(x => x.id == id).providerName;
    });
  }

  // load recommendation data of location
  loadDataOfLocation() {
    this._dataService
      .getNoAu(
        "/Recommendation/GetRecommendLocation?isDefault=false"
      )
      .subscribe((response: any) => {
        this.locations = response;
      });
  }
  sliderConfigPopular = { "slidesToShow": 4, "slidesToScroll": 1, "arrows": true };

  //  get all category
  getAllCategory() {
    this._dataService.getNoAu("/CategoryManagement/GetAllCategory").subscribe((response: any) => {
      this.category = response;
      console.log("fking thing "+this.category);
    });
  }

  // get recommend data of news of provider 
  getAllNews() {
    this._dataService.getNoAu("/Recommendation/GetRecommendNews?isDefault=false")
      .subscribe((response: any) => {
        this.news = response;
        console.log(this.news)
      });
  }

  // get all hot service - recommend data service
  getRecommendService(){
    this._dataService.getNoAu("/Recommendation/GetRecommendService?isDefault=false")
    .subscribe((response: any) =>{
      this.hotService = response;
    });
  }
}
