import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { PurchaseBillService } from '../../core/services/purchase-bill.service';
import { Location, PurchaseBillTableRow } from '../../core/models/purchase-bill.models';

const ALLOWED_ITEMS = ['Mango', 'Apple', 'Banana', 'Orange', 'Grapes', 'Kiwi', 'Strawberry'];

function allowedItemValidator(control: AbstractControl): ValidationErrors | null {
  if (!control.value) return null; // let required validator handle empties
  return ALLOWED_ITEMS.includes(control.value) ? null : { invalidItem: true };
}

@Component({
  selector: 'app-purchase-bill',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './purchase-bill.html',
  styleUrl: './purchase-bill.scss'
})
export class PurchaseBillComponent implements OnInit {
  billForm: FormGroup;
  
  locations: Location[] = [];
  locationsLoading = true;
  locationsError = '';
  
  isAdding = false;
  backendError = '';
  
  tableRows: PurchaseBillTableRow[] = [];
  allowedItems = ALLOWED_ITEMS;
  
  localTotalCost = 0;
  localTotalSelling = 0;

  constructor(private fb: FormBuilder, private purchaseBillService: PurchaseBillService) {
    this.billForm = this.fb.group({
      item: ['', [Validators.required, allowedItemValidator]],
      locationCode: ['', [Validators.required]],
      standardCost: [null, [Validators.required, Validators.min(0.01)]],
      standardPrice: [null, [Validators.required, Validators.min(0.01)]],
      quantity: [null, [Validators.required, Validators.min(1), Validators.pattern('^[0-9]+$')]],
      discountPercentage: [0, [Validators.required, Validators.min(0), Validators.max(100)]]
    });
  }

  ngOnInit(): void {
    this.loadLocations();
    
    // Subscribe to form changes to update read-only totals locally
    this.billForm.valueChanges.subscribe(val => {
      this.calculateLocalTotals(val);
    });
  }

  loadLocations(): void {
    this.purchaseBillService.getLocations().subscribe({
      next: (data) => {
        this.locations = data;
        this.locationsLoading = false;
      },
      error: () => {
        this.locationsLoading = false;
        this.locationsError = 'Failed to load batch locations. Please refresh.';
      }
    });
  }

  calculateLocalTotals(val: any): void {
    const cost = Number(val.standardCost);
    const qty = Number(val.quantity);
    const discount = Number(val.discountPercentage) || 0;
    const price = Number(val.standardPrice);

    if (!isNaN(cost) && !isNaN(qty) && cost > 0 && qty > 0) {
      const baseCost = cost * qty;
      const discountAmount = baseCost * (discount / 100);
      this.localTotalCost = baseCost - discountAmount;
    } else {
      this.localTotalCost = 0;
    }

    if (!isNaN(price) && !isNaN(qty) && price > 0 && qty > 0) {
      this.localTotalSelling = price * qty;
    } else {
      this.localTotalSelling = 0;
    }
  }
  
  get totalItems(): number {
    return this.tableRows.length;
  }

  get totalQuantity(): number {
    return this.tableRows.reduce((sum, row) => sum + row.quantity, 0);
  }

  onAdd(): void {
    if (this.billForm.invalid || this.isAdding) {
      this.billForm.markAllAsTouched();
      return;
    }

    this.isAdding = true;
    this.backendError = '';

    const request = this.billForm.value;

    this.purchaseBillService.calculatePurchaseBill(request).subscribe({
      next: (response) => {
        this.isAdding = false;
        
        // Find location name for table display
        const locationName = this.locations.find(l => l.locationCode === response.locationCode)?.locationName || response.locationCode;
        
        // Push the backend-calculated exact values to the table
        this.tableRows.push({ ...response, locationName });
        
        // Sensible form reset: clear text/numbers, keep discount at 0, reset untouched state
        this.billForm.reset({
          item: '',
          locationCode: request.locationCode, // Usually helpful to keep same batch selected
          standardCost: null,
          standardPrice: null,
          quantity: null,
          discountPercentage: 0
        });
      },
      error: (err: HttpErrorResponse) => {
        this.isAdding = false;
        if (err.status === 400 && err.error?.title) {
          this.backendError = 'Validation failed on the server.';
        } else {
          this.backendError = err.error?.message || 'An unexpected error occurred while adding the item.';
        }
      }
    });
  }
}
