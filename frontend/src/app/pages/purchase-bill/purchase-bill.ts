import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { PurchaseBillService } from '../../core/services/purchase-bill.service';
import { AuthService } from '../../core/services/auth.service';
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
  
  // Custom Autocomplete State
  showItemDropdown = false;
  filteredItems: string[] = [];
  
  localTotalCost = 0;
  localTotalSelling = 0;

  private readonly STORAGE_KEY = 'billflow_purchase_bill_rows';

  constructor(
    private fb: FormBuilder, 
    private purchaseBillService: PurchaseBillService,
    private authService: AuthService,
    private router: Router
  ) {
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
    this.restorePersistedRows();
    
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

  restorePersistedRows(): void {
    const saved = localStorage.getItem(this.STORAGE_KEY);
    if (saved) {
      try {
        this.tableRows = JSON.parse(saved);
      } catch (e) {
        console.error('Failed to parse saved bill rows');
      }
    }
  }

  filterItems(): void {
    const val = this.billForm.get('item')?.value?.toLowerCase() || '';
    this.filteredItems = this.allowedItems.filter(i => i.toLowerCase().includes(val));
  }

  selectItem(option: string): void {
    this.billForm.patchValue({ item: option });
    this.billForm.get('item')?.markAsTouched();
    this.showItemDropdown = false;
  }

  hideItemDropdown(): void {
    // Delay hiding to allow the click event on a suggestion to process
    setTimeout(() => {
      this.showItemDropdown = false;
    }, 200);
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
        
        const locationName = this.locations.find(l => l.locationCode === response.locationCode)?.locationName || response.locationCode;
        
        this.tableRows.push({ ...response, locationName });
        localStorage.setItem(this.STORAGE_KEY, JSON.stringify(this.tableRows));
        
        this.billForm.reset({
          item: '',
          locationCode: request.locationCode, 
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

  onClearItems(): void {
    this.tableRows = [];
    localStorage.removeItem(this.STORAGE_KEY);
  }

  onLogout(): void {
    this.authService.logout().subscribe({
      next: () => this.router.navigate(['/login']),
      error: () => this.router.navigate(['/login'])
    });
  }
}
