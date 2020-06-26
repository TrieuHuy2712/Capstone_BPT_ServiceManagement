import { Component, OnInit, ViewChild } from '@angular/core';
import { DataService } from 'src/app/core/services/data.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { SystemConstants } from '../../core/common/system,constants';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  public isLoggedIn= false;
  locations: any[];
  public category: any[];
  public news: any[];
  public hotService: any[];
  public recommendService: any[];
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
    this.getUserRecommendationService();
  }

  // slider 1
  slides = [
    { img: "../../../assets/images/banner-1.jpg" },
    { img: "../../../assets/images/banner-2.jpg" },
    { img: "../../../assets/images/banner-3.jpg" }
  ];
  slideConfig = { "slidesToShow": 1, "slidesToScroll": 1, "arrows": false, "autoplay": true, "autoplaySpeed": 500 };
  slideConfig2 = { "slidesToShow": 5, "slidesToScroll": 1, "arrows": true, "autoplay": false };

  //  location slider

  slideConfig3 = { "slidesToShow": 4, "slidesToScroll": 1, "arrows": false, "autoplay": false };

  // slider config of service
  sliderConfigOfService = { "slidesToShow": 4, "slidesToScroll": 1, "arrows": false, "autoplay": false };

  afterChange(e) {
    console.log('afterChange');
  }

  // show detail of news of provider
  showAddModal(id: any) {
    this.modalEditProfile.show();
    this._dataService.getNoAu("/ProviderNews/GetAllPagingProviderNews?isAdminPage=true&filter=1")
      .subscribe((response: any) => {
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
      console.log("fking thing " + this.category);
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
  getRecommendService() {
    this._dataService.getNoAu("/Recommendation/GetRecommendService?isDefault=false")
      .subscribe((response: any) => {
        this.hotService = response;
      });
  }

  getUserRecommendationService() {
    if (localStorage.getItem(SystemConstants.const_username) != null) {
      this.isLoggedIn = true;
    }
    this._dataService.get('/Recommendation/GetRecommendUserService').subscribe((response: any) => {
      this.recommendService = response;
    });
  }
}
