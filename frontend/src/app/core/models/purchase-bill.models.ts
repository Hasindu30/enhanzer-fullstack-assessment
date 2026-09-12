export interface Location {
  locationCode: string;
  locationName: string;
}

export interface PurchaseBillCalculateRequest {
  item: string;
  locationCode: string;
  standardCost: number;
  standardPrice: number;
  quantity: number;
  discountPercentage: number;
}

export interface PurchaseBillCalculateResponse {
  item: string;
  locationCode: string;
  standardCost: number;
  standardPrice: number;
  quantity: number;
  discountPercentage: number;
  totalCost: number;
  totalSelling: number;
}

export interface PurchaseBillTableRow extends PurchaseBillCalculateResponse {
  locationName: string;
}
