import { Component, OnInit } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MixinService, MixinEntity } from '../../core/services/mixin.service';

@Component({
  selector: 'app-mixin-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule
  ],
  templateUrl: './mixin-list.component.html',
  styleUrls: ['./mixin-list.component.scss']
})
export class MixinListComponent implements OnInit {
  displayedColumns: string[] = ['name', 'displayName', 'description', 'createdAt', 'actions'];
  mixins: MixinEntity[] = [];
  loading = false;

  constructor(private mixinService: MixinService) {}

  ngOnInit(): void {
    this.loadMixins();
  }

  loadMixins(): void {
    this.loading = true;
    this.mixinService.getAll().subscribe({
      next: (mixins) => {
        this.mixins = mixins;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading mixins:', error);
        this.loading = false;
      }
    });
  }

  onEdit(mixin: MixinEntity): void {
    // TODO: Open edit dialog
    console.log('Edit mixin:', mixin);
  }

  onDelete(mixin: MixinEntity): void {
    // TODO: Open delete confirmation dialog
    console.log('Delete mixin:', mixin);
  }

  onCreate(): void {
    // TODO: Open create dialog
    console.log('Create new mixin');
  }
}
