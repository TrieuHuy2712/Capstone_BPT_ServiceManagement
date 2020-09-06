import { Injectable } from "@angular/core";
import { LanguageService } from './language.service';

@Injectable()
export class TranslationSet {
  public languange: string;
  public values: { [key: string]: string } = {};
}
@Injectable()
export class TranslationService {
  public languages = ["vn", "en"];

  public language = "vn";

  private dictionary: { [key: string]: TranslationSet } = {
    vn: {
      languange: "vn",
      values: {
        add: "Thêm",
        name: "Tên",
        description: "Mô tả",
        search: "Tìm !",
        typeAFunction: "Gõ tên quyền...",
        addAndEdit: "Thêm / Sửa",
        nameOfGroup: "Tên nhóm",
        nameWarning: "Bạn phải nhập tên ít nhất 3 ký tự",
        descriptionWarning: "Bạn phải nhập mô tả",
        cancel: "Hủy bỏ",
        update: "Cập nhật",
        listOfUser: "Danh sách người dùng",
        functionId:"Mã chức năng",
        functionIdWarning: "Bạn phải nhập mã chức năng !",
        functionName: "Tên chức năng",
        vietnameseName: "Tên tiếng việt",
        link: "Đường dẫn",
        linkWarning: "Bạn phải nhập đường dẫn",
        order: "Thứ tự",
        orderWarning: "Bạn phải nhập thứ tự",
        rootFunction: "Chức năng cha",
        selectFunction: "--Chọn chức năng cha--",
        status: "Trạng thái",
        active: "Kích hoạt",
        decentralization: "Phân quyền cho chức năng",
        roleName: "Tên quyền",
        yourRole: "Quyền được làm",
        read: "Xem",
        edit: "Chỉnh sửa",
        delete: "Xóa",
        account: "Tài khoản",
        fullName: "Họ và tên",
        email: "Thư điện tử",
        avatar: "Hình đại diện",
        action: "Tác vụ",
        accountWarning: "Bạn phải nhập tài khoản !",
        password: "Mật khẩu",
        passwordWaring: "Bạn phải nhập tên ít nhất 6 ký tự !",
        passwordConfirm: "Xác nhận mật khẩu",
        passwordConfirmWarning: "Xác nhận mật khẩu sai ",
        emailWarning: "Bạn phải nhập Email !",
        address: "Địa chỉ",
        role: "Thuộc nhóm ",
        home: "Trang chủ",
        contact: "Liên hệ",
      }
    },
    en: {
      languange: "en",
      values: {
        add: "Add",
        name: "Name",
        description: "Description",
        search: "Search !",
        typeAFunction: "Type a name...",
        addAndEdit: "Add / Edit",
        nameOfGroup: "Name of group",
        nameWarning: "You must enter at least 3 characters",
        descriptionWarning: "You must enter description",
        cancel: "Cancel",
        update: "Update",
        listOfUser: "List of user",
        functionId:"Function id",
        functionIdWarning: "You must enter function id !",
        functionName: "Function name",
        vietnameseName: "Vietnamese name",
        link: "Link",
        linkWarning: "You must enter the link",
        order: "Order",
        orderWarning: "Bạn phải nhập thứ tự",
        rootFunction: "Root function",
        selectFunction: "--Select root function--",
        status: "Status",
        active: "Active",
        decentralization: "decentralization for a function",
        roleName: "Role name",
        yourRole: "permission to do",
        read: "View",
        edit: "Edit",
        delete: "Delete",
        account: "Account",
        fullName: "Fullname",
        email: "Email",
        avatar: "Avatar",
        action: "Action",
        accountWarning: "You must enter account name !",
        password: "Password",
        passwordWaring: "You must enter at least 6 characters !",
        passwordConfirm: "Password confirm",
        passwordConfirmWarning: "Your password confirm is not the same",
        emailWarning: "You must enter Email !",
        address: "Address",
        role: "Role",
        home: "Home",
        contact: "Contact",

      }
    }
  };

  constructor(private languageService: LanguageService) {}

  translate(key: string): string {
    if (this.languageService.getLanguage() == "vn") {
      return this.dictionary[this.languageService.getLanguage()].values[key];
    }else{
        return this.dictionary["en"].values[key];
    }
  }
}