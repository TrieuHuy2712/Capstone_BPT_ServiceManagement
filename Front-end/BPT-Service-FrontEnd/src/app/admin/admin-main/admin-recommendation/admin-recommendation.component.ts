import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalDirective, TypeaheadMatch } from 'ngx-bootstrap';
import { DataService } from '../../../core/services/data.service';
import { NotificationService } from '../../../core/services/notification.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { MessageConstants } from '../../../core/common/message.constants';

enum TypeRecommend {
  News = 0,
  Location = 1,
  Service = 2
}

@Component({
  selector: 'app-admin-recommendation',
  templateUrl: './admin-recommendation.component.html',
})
export class AdminRecommendationComponent implements OnInit {
  @ViewChild('modalAddEdit', { static: false })
  public modalAddEdit: ModalDirective;
  public TypeRecommend = TypeRecommend;
  public currentRecommendation = TypeRecommend.News;
  public typeAddRecommendation = TypeRecommend.News;
  // Declare list Recommend
  public listRecommendNews: any;
  public listRecommendLocation: any;
  public listRecommendService: any;
  // Declare flat
  public isAdd: boolean;
  // Declare  Permission
  public permission: any;
  public functionId: 'RECOMMENDATION';
  public entity: any;
  // Declare another list
  public listNews: any;
  public listLocation: any;
  public listService: any;
  // Declare seleted item
  public selectedNews: any;
  public selectedLocation: any;
  public selectedService: any;
  constructor(
    private dataService: DataService,
    private notificationService: NotificationService,
    private spinnerService: Ng4LoadingSpinnerService
  ) { }

  ngOnInit() {
    this.permission = {
      canCreate: false,
      canDelete: false,
      canUpdate: false,
      canRead: false
    };

    this.getRecommendLocation(false);
    this.getRecommendNews(false);
    this.getRecommendService(false);
    this.getAllInformation();
  }

  showAddModal() {
    this.isAdd = true;
    this.entity = {};
    this.modalAddEdit.show();
  }


  showEditModal(type: TypeRecommend, id: any) {
    this.isAdd = false
    if (type == TypeRecommend.News) {
      this.selectedNews = undefined;
      this.entity = this.listRecommendNews[id];
      this.entity.selectedLocation = this.listRecommendNews[id].title;
      this.entity.idType = this.listRecommendNews[id].idNews;
    } else if (type == TypeRecommend.Location) {
      this.selectedLocation = undefined;
      this.entity = this.listRecommendLocation[id];
      this.entity.selectedLocation = this.listRecommendLocation[id].nameLocation;
      this.entity.idType = this.listRecommendLocation[id].idLocation;
    } else if (type == TypeRecommend.Service) {
      this.selectedService = undefined;
      this.entity = this.listRecommendService[id];
      this.entity.selectedService = this.listRecommendService[id].nameService;
      this.entity.idType = this.listRecommendService[id].idService;
    }
    this.modalAddEdit.show();
  }

  filterChanged(typeRecommend: any) {
    console.log(this.currentRecommendation);
    this.currentRecommendation = typeRecommend;
  }

  getRecommendLocation(isDefault: boolean) {
    this.dataService
      .get(
        '/Recommendation/GetRecommendLocation?isDefault=' + isDefault
      )
      .subscribe((response: any) => {
        this.listRecommendLocation = response;
        this.loadPermission();
      });
  }

  getRecommendNews(isDefault: boolean) {
    this.spinnerService.show();
    this.dataService
      .get(
        '/Recommendation/GetRecommendNews?isDefault=' + isDefault
      )
      .subscribe((response: any) => {
        this.listRecommendNews = response;
        this.spinnerService.hide();
      });
  }

  getRecommendService(isDefault: boolean) {
    this.dataService
      .get(
        '/Recommendation/GetRecommendService?isDefault=' + isDefault
      )
      .subscribe((response: any) => {
        this.listRecommendService = response;
      });
  }

  loadPermission() {
    this.dataService
      .get(
        '/PermissionManager/GetAllPermission/' + this.functionId
      )
      .subscribe((response: any) => {
        this.permission = response;
      });
  }

  getAllInformation() {
    // Get dat location
    this.dataService
      .get(`/LocationManagement/GetAllPaging?page=0&pageSize=0`).subscribe((response: any) => {
        this.listLocation = response.results;
        this.listLocation.forEach(element => {
          element.city = element.city + '_' + element.province;
        });
      });
    // Get all news
    this.dataService
      .get(`/ProviderNews/GetAllPagingProviderNews?page=0&pageSize=0`).subscribe((response: any) => {
        this.listNews = response.results;
      });
    // Get all service
    this.dataService
      .get(`/Service/getAllPagingPostService?page=0&pageSize=0`).subscribe((response: any) => {
        this.listService = response.results;
      });
  }
  onSelectIdType(type: TypeRecommend, event: TypeaheadMatch): void {
    if (type == TypeRecommend.Location) {
      this.selectedLocation = event.item;
    } else if (type == TypeRecommend.News) {
      this.selectedNews = event.item;
    } else if (type == TypeRecommend.Service) {
      this.selectedService = event.item;
    }
  }

  saveData() {
    if (this.currentRecommendation == TypeRecommend.Location) {
      // console.log(this.selectedLocation);
      // console.log(this.entity.idType);
      if (this.selectedLocation !== undefined) {
        this.entity.idType = this.selectedLocation.id;
      }
      this.saveLocation();
    } else if (this.currentRecommendation == TypeRecommend.News) {
      if (this.selectedNews !== undefined) {
        this.entity.idType = this.selectedNews.id;
      }
      this.saveNews();

    } else if (this.currentRecommendation == TypeRecommend.Service) {
      if (this.selectedService !== undefined) {
        this.entity.idType = this.selectedService.id;
      }
      this.saveService();

    }
  }

  saveLocation() {
    this.dataService.post('/Recommendation/AddLocationRecommend', this.entity).subscribe(
      (response: any) => {
        if (response.isValid === true) {
          this.getRecommendLocation(false);
          this.notificationService.printSuccessMessage(
            MessageConstants.CREATED_OK_MSG
          );
          this.modalAddEdit.hide();
        } else {
          this.notificationService.printErrorMessage(
            MessageConstants.CREATED_FAIL_MSG
          );
        }
        this.spinnerService.hide();
      },
      error => this.dataService.handleError(error)
    );
  }

  saveNews() {
    this.dataService.post('/Recommendation/AddNewsRecommend', this.entity).subscribe(
      (response: any) => {
        if (response.isValid === true) {
          this.getRecommendNews(false);
          this.notificationService.printSuccessMessage(
            MessageConstants.CREATED_OK_MSG
          );
          this.modalAddEdit.hide();
        } else {
          this.notificationService.printErrorMessage(
            MessageConstants.CREATED_FAIL_MSG
          );
        }
        this.spinnerService.hide();
      },
      error => this.dataService.handleError(error)
    );
  }



  saveService() {
    this.dataService.post('/Recommendation/AddServiceRecommend', this.entity).subscribe(
      (response: any) => {
        if (response.isValid === true) {
          this.getRecommendService(false);
          this.notificationService.printSuccessMessage(
            MessageConstants.CREATED_OK_MSG
          );
          this.modalAddEdit.hide();
        } else {
          this.notificationService.printErrorMessage(
            MessageConstants.CREATED_FAIL_MSG
          );
        }
        this.spinnerService.hide();
      },
      error => this.dataService.handleError(error)
    );
  }

  setDefaultRecommend() {
    if (this.currentRecommendation == TypeRecommend.Location) {
      this.getRecommendLocation(true);
    } else if (this.currentRecommendation == TypeRecommend.News) {
      this.getRecommendNews(true);
    } else if (this.currentRecommendation == TypeRecommend.Service) {
      this.getRecommendService(true);
    }
  }

  deleteItem(type: TypeRecommend, idRole: any, id: any) {
    this.notificationService.printConfirmationDialog(
      MessageConstants.CONFIRM_DELETE_MSG,
      () => this.deleteItemConfirm(type, idRole, id)
    );
  }
  deleteItemConfirm(type: TypeRecommend, idRole: any, id: any) {
    this.spinnerService.show();
    this.dataService
      .delete('/Recommendation/DeleteRecommend', 'id', idRole)
      .subscribe((response: any) => {
        if (response.isValid == true) {
          if (type == TypeRecommend.Location) {
            this.listRecommendLocation.splice(id, 1);

          } else if (type == TypeRecommend.News) {
            this.listRecommendNews.splice(id, 1);

          } else if (type == TypeRecommend.Service) {
            this.listRecommendNews.splice(id, 1);

          }
          this.notificationService.printSuccessMessage(
            MessageConstants.DELETED_OK_MSG
          );
        } else {
          this.notificationService.printErrorMessage(
            MessageConstants.DELETED_FAIL_MSG
          );
        }
        this.spinnerService.hide();
      });
  }


}
