import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { MixinService, MixinEntity } from '../../core/services/mixin.service';

@Component({
  selector: 'app-mixin-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule
  ],
  templateUrl: './mixin-form.component.html',
  styleUrls: ['./mixin-form.component.scss']
})
export class MixinFormComponent implements OnInit {
  mixinForm: FormGroup;
  isEditMode = false;
  mixinId: string | null = null;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private mixinService: MixinService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.mixinForm = this.fb.group({
      name: ['', [Validators.required, Validators.pattern(/^[a-zA-Z_][a-zA-Z0-9_]*$/)]],
      displayName: ['', Validators.required],
      description: [''],
      schema: [{}]
    });
  }

  ngOnInit(): void {
    this.mixinId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.mixinId;

    if (this.isEditMode && this.mixinId) {
      this.loadMixin(this.mixinId);
    }
  }

  loadMixin(id: string): void {
    this.loading = true;
    this.mixinService.getById(id).subscribe({
      next: (mixin) => {
        this.mixinForm.patchValue({
          name: mixin.name,
          displayName: mixin.displayName,
          description: mixin.description,
          schema: mixin.schema
        });
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading mixin:', error);
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.mixinForm.valid) {
      this.loading = true;
      const mixinData = this.mixinForm.value;

      const operation = this.isEditMode && this.mixinId
        ? this.mixinService.update(this.mixinId, mixinData)
        : this.mixinService.create(mixinData);

      operation.subscribe({
        next: () => {
          this.router.navigate(['/schema/mixins']);
        },
        error: (error) => {
          console.error('Error saving mixin:', error);
          this.loading = false;
        }
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/schema/mixins']);
  }

  getTitle(): string {
    return this.isEditMode ? 'Редактирование миксина' : 'Создание миксина';
  }
}
