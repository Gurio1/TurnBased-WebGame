import { CommonModule } from '@angular/common';
import { CdkDragDrop, DragDropModule } from '@angular/cdk/drag-drop';
import { Component, OnInit, inject } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { forkJoin } from 'rxjs';
import { AdminBlueprintsService } from './services/admin-blueprints.service';
import { EquipmentBlueprintDetail } from './models/equipment-blueprint-detail';
import { EquipmentBlueprintCreatedResponse } from './models/equipment-blueprint-created-response';
import { EquipmentBlueprintEquipmentCatalogItem } from './models/equipment-blueprint-equipment-catalog-item';
import { EquipmentBlueprintSaveRequest } from './models/equipment-blueprint-save-request';
import { EquipmentBlueprintStatCatalogItem } from './models/equipment-blueprint-stat-catalog-item';
import { EquipmentBlueprintSummary } from './models/equipment-blueprint-summary';
import { isCriticalPercentStat } from '../../../core/utils/critical-stat-display';

@Component({
  selector: 'app-admin-blueprints-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DragDropModule],
  templateUrl: './admin-blueprints-page.component.html',
  styleUrl: './admin-blueprints-page.component.scss',
})
export class AdminBlueprintsPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly adminBlueprintsService = inject(AdminBlueprintsService);

  readonly searchControl = new FormControl('', { nonNullable: true });
  readonly linkedEquipmentSearchControl = new FormControl('', { nonNullable: true });
  readonly statToAddControl = new FormControl('', { nonNullable: true });

  readonly blueprintForm = this.fb.group({
    equipmentId: this.fb.control('', { nonNullable: true, validators: [Validators.required] }),
    stats: this.fb.array([]),
    countWeights: this.fb.array([]),
  });

  blueprints: EquipmentBlueprintSummary[] = [];
  equipmentCatalog: EquipmentBlueprintEquipmentCatalogItem[] = [];
  statCatalog: EquipmentBlueprintStatCatalogItem[] = [];

  selectedBlueprintId: string | null = null;
  isLoading = true;
  isSaving = false;
  isDeleting = false;
  formSubmitted = false;
  loadError: string | null = null;
  submitError: string | null = null;
  successMessage: string | null = null;

  ngOnInit(): void {
    this.loadBlueprintData();
  }

  get statsArray(): FormArray {
    return this.blueprintForm.get('stats') as FormArray;
  }

  get countWeightsArray(): FormArray {
    return this.blueprintForm.get('countWeights') as FormArray;
  }

  get filteredEquipmentCatalog(): EquipmentBlueprintEquipmentCatalogItem[] {
    const searchTerm = this.searchControl.value.trim().toLowerCase();

    if (!searchTerm) {
      return this.equipmentCatalog;
    }

    return this.equipmentCatalog.filter((item) => {
      return (
        item.name.toLowerCase().includes(searchTerm) ||
        item.slot.toLowerCase().includes(searchTerm) ||
        item.equipmentId.toLowerCase().includes(searchTerm)
      );
    });
  }

  get selectedEquipment(): EquipmentBlueprintEquipmentCatalogItem | null {
    const equipmentId = this.blueprintForm.get('equipmentId')?.value as string;
    if (!equipmentId) {
      return null;
    }

    return (
      this.equipmentCatalog.find((item) => item.equipmentId === equipmentId) ??
      null
    );
  }

  get canDeleteSelected(): boolean {
    return this.selectedBlueprintId !== null && !this.isDeleting;
  }

  get filteredBlueprints(): EquipmentBlueprintSummary[] {
    const searchTerm = this.linkedEquipmentSearchControl.value.trim().toLowerCase();

    if (!searchTerm) {
      return this.blueprints;
    }

    return this.blueprints.filter((blueprint) => {
      return (
        blueprint.equipmentName.toLowerCase().includes(searchTerm) ||
        blueprint.equipmentSlot.toLowerCase().includes(searchTerm) ||
        blueprint.equipmentId.toLowerCase().includes(searchTerm)
      );
    });
  }

  get isFormInvalid(): boolean {
    return (
      !this.blueprintForm.get('equipmentId')?.value ||
      this.statsArray.length === 0 ||
      this.hasDuplicateStatKeys() ||
      this.hasInvalidRanges() ||
      this.hasInvalidWeights()
    );
  }

  get selectedStatKeys(): Set<string> {
    return new Set(
      this.statsArray.controls
        .map((control) => control.get('statKey')?.value as string)
        .filter((value) => !!value)
    );
  }

  loadBlueprintData(selectBlueprintId: string | null = this.selectedBlueprintId): void {
    this.isLoading = true;
    this.loadError = null;

    forkJoin({
      blueprints: this.adminBlueprintsService.getBlueprints(),
      equipmentCatalog: this.adminBlueprintsService.getEquipmentCatalog(),
      statCatalog: this.adminBlueprintsService.getStatCatalog(),
    }).subscribe({
      next: ({ blueprints, equipmentCatalog, statCatalog }) => {
        this.blueprints = blueprints;
        this.equipmentCatalog = equipmentCatalog;
        this.statCatalog = statCatalog;

        if (selectBlueprintId) {
          this.openBlueprint(selectBlueprintId);
          return;
        }

        this.startCreate();
        this.isLoading = false;
      },
      error: (error: Error) => {
        this.loadError = error.message;
        this.isLoading = false;
      },
    });
  }

  startCreate(): void {
    this.selectedBlueprintId = null;
    this.successMessage = null;
    this.submitError = null;
    this.formSubmitted = false;
    this.statToAddControl.setValue('');

    this.blueprintForm.reset({ equipmentId: '' });
    this.clearFormArray(this.statsArray);
    this.clearFormArray(this.countWeightsArray);
    this.syncCountWeightControls();
  }

  openBlueprint(blueprintId: string): void {
    this.selectedBlueprintId = blueprintId;
    this.submitError = null;
    this.successMessage = null;

    this.adminBlueprintsService.getBlueprint(blueprintId).subscribe({
      next: (blueprint) => {
        this.patchBlueprintForm(blueprint);
        this.isLoading = false;
      },
      error: (error: Error) => {
        this.loadError = error.message;
        this.isLoading = false;
      },
    });
  }

  onEquipmentDropped(event: CdkDragDrop<EquipmentBlueprintEquipmentCatalogItem[]>): void {
    const equipment = event.item.data as EquipmentBlueprintEquipmentCatalogItem;
    this.attachEquipment(equipment);
  }

  attachEquipment(equipment: EquipmentBlueprintEquipmentCatalogItem): void {
    if (!this.canAttachEquipment(equipment)) {
      return;
    }

    this.blueprintForm.patchValue({ equipmentId: equipment.equipmentId });
    this.submitError = null;
  }

  clearEquipment(): void {
    this.blueprintForm.patchValue({ equipmentId: '' });
  }

  addSelectedStat(): void {
    const statKey = this.statToAddControl.value;
    if (!statKey) {
      return;
    }

    if (this.selectedStatKeys.has(statKey)) {
      this.submitError = 'Each stat can only be selected once.';
      return;
    }

    this.statsArray.push(
      this.fb.group({
        statKey: this.fb.control(statKey, { nonNullable: true, validators: [Validators.required] }),
        minValue: this.fb.control(0, { nonNullable: true, validators: [Validators.required] }),
        maxValue: this.fb.control(0, { nonNullable: true, validators: [Validators.required] }),
      })
    );

    this.statToAddControl.setValue('');
    this.submitError = null;
    this.syncCountWeightControls();
  }

  removeStat(index: number): void {
    this.statsArray.removeAt(index);
    this.syncCountWeightControls();
  }

  onStatSelectionChanged(): void {
    this.submitError = null;
  }

  saveBlueprint(): void {
    this.formSubmitted = true;
    this.submitError = null;
    this.successMessage = null;

    if (this.isFormInvalid) {
      this.submitError = this.resolveValidationMessage();
      return;
    }

    const request = this.buildSaveRequest();
    if (!request) {
      this.submitError = 'Blueprint data is incomplete.';
      return;
    }

    this.isSaving = true;
    const isCreating = this.selectedBlueprintId === null;

    if (isCreating) {
      this.adminBlueprintsService.createBlueprint(request).subscribe({
        next: (created: EquipmentBlueprintCreatedResponse) => {
          this.isSaving = false;
          this.successMessage = 'Blueprint created.';
          this.loadBlueprintData(created.id);
        },
        error: (error: Error) => {
          this.isSaving = false;
          this.submitError = error.message;
        },
      });

      return;
    }

    const blueprintId = this.selectedBlueprintId;
    if (!blueprintId) {
      this.isSaving = false;
      this.submitError = 'Blueprint id is missing.';
      return;
    }

    this.adminBlueprintsService.updateBlueprint(blueprintId, request).subscribe({
      next: () => {
        this.isSaving = false;
        this.successMessage = 'Blueprint updated.';
        this.reloadAfterSave(request.equipmentId);
      },
      error: (error: Error) => {
        this.isSaving = false;
        this.submitError = error.message;
      },
    });
  }

  deleteBlueprint(): void {
    if (!this.selectedBlueprintId || !window.confirm('Delete this blueprint?')) {
      return;
    }

    this.isDeleting = true;
    this.submitError = null;
    this.successMessage = null;

    this.adminBlueprintsService.deleteBlueprint(this.selectedBlueprintId).subscribe({
      next: () => {
        this.isDeleting = false;
        this.successMessage = 'Blueprint deleted.';
        this.loadBlueprintData(null);
      },
      error: (error: Error) => {
        this.isDeleting = false;
        this.submitError = error.message;
      },
    });
  }

  canAttachEquipment(equipment: EquipmentBlueprintEquipmentCatalogItem): boolean {
    return (
      !equipment.assignedBlueprintId ||
      equipment.assignedBlueprintId === this.selectedBlueprintId
    );
  }

  getEquipmentImagePath(imageUrl: string): string {
    return `${imageUrl}.png`;
  }

  getStatName(statKey: string): string {
    return (
      this.statCatalog.find((item) => item.key === statKey)?.name ??
      statKey
    );
  }

  getStatOptionsForRow(index: number): EquipmentBlueprintStatCatalogItem[] {
    const currentKey = this.statsArray.at(index)?.get('statKey')?.value as string;

    return this.statCatalog.filter((item) => {
      return item.key === currentKey || !this.selectedStatKeys.has(item.key);
    });
  }

  getStatInputStep(statKey: string | null | undefined): number {
    return this.isCriticalStatKey(statKey) ? 1 : 0.01;
  }

  isEquipmentSelected(equipmentId: string): boolean {
    return this.blueprintForm.get('equipmentId')?.value === equipmentId;
  }

  trackBlueprint(_index: number, blueprint: EquipmentBlueprintSummary): string {
    return blueprint.id;
  }

  trackEquipment(
    _index: number,
    equipment: EquipmentBlueprintEquipmentCatalogItem
  ): string {
    return equipment.equipmentId;
  }

  trackStat(_index: number, stat: EquipmentBlueprintStatCatalogItem): string {
    return stat.key;
  }

  private patchBlueprintForm(blueprint: EquipmentBlueprintDetail): void {
    this.formSubmitted = false;
    this.blueprintForm.patchValue({ equipmentId: blueprint.equipmentId });
    this.clearFormArray(this.statsArray);

    blueprint.stats.forEach((stat) => {
      this.statsArray.push(
        this.fb.group({
          statKey: this.fb.control(stat.statKey, {
            nonNullable: true,
            validators: [Validators.required],
          }),
          minValue: this.fb.control(stat.minValue, {
            nonNullable: true,
            validators: [Validators.required],
          }),
          maxValue: this.fb.control(stat.maxValue, {
            nonNullable: true,
            validators: [Validators.required],
          }),
        })
      );
    });

    this.syncCountWeightControls(blueprint.countWeights);
  }

  private syncCountWeightControls(
    existingWeights: { count: number; weight: number }[] = []
  ): void {
    const preservedWeights = new Map<number, number>(
      this.countWeightsArray.controls.map((control) => [
        Number(control.get('count')?.value),
        Number(control.get('weight')?.value),
      ])
    );

    existingWeights.forEach((entry) => preservedWeights.set(entry.count, entry.weight));

    this.clearFormArray(this.countWeightsArray);

    for (let count = 1; count <= this.statsArray.length; count += 1) {
      this.countWeightsArray.push(
        this.fb.group({
          count: this.fb.control(count, { nonNullable: true, validators: [Validators.required] }),
          weight: this.fb.control(preservedWeights.get(count) ?? (count === 1 ? 1 : 0), {
            nonNullable: true,
            validators: [Validators.required, Validators.min(0)],
          }),
        })
      );
    }
  }

  private resolveValidationMessage(): string {
    if (!this.blueprintForm.get('equipmentId')?.value) {
      return 'Attach one equipment item to the blueprint.';
    }

    if (this.statsArray.length === 0) {
      return 'Add at least one stat to the blueprint.';
    }

    if (this.hasDuplicateStatKeys()) {
      return 'Each stat can only be selected once.';
    }

    if (this.hasInvalidRanges()) {
      return 'Each stat range must have a minimum value less than or equal to the maximum value.';
    }

    if (this.hasInvalidWeights()) {
      return 'Count weights must stay non-negative and at least one weight must be greater than zero.';
    }

    return 'Review the blueprint form and try again.';
  }

  private hasDuplicateStatKeys(): boolean {
    const statKeys = this.statsArray.controls
      .map((control) => control.get('statKey')?.value as string)
      .filter((value) => !!value);

    return new Set(statKeys).size !== statKeys.length;
  }

  private hasInvalidRanges(): boolean {
    return this.statsArray.controls.some((control) => {
      const minValue = Number(control.get('minValue')?.value);
      const maxValue = Number(control.get('maxValue')?.value);
      return Number.isNaN(minValue) || Number.isNaN(maxValue) || minValue > maxValue;
    });
  }

  private hasInvalidWeights(): boolean {
    let totalWeight = 0;

    for (const control of this.countWeightsArray.controls) {
      const weight = Number(control.get('weight')?.value);
      if (Number.isNaN(weight) || weight < 0) {
        return true;
      }

      totalWeight += weight;
    }

    return totalWeight <= 0;
  }

  private buildSaveRequest(): EquipmentBlueprintSaveRequest | null {
    const equipmentId = this.blueprintForm.get('equipmentId')?.value as string;
    if (!equipmentId) {
      return null;
    }

    return {
      equipmentId,
      stats: this.statsArray.controls.map((control) => ({
        statKey: control.get('statKey')?.value as string,
        minValue: Number(control.get('minValue')?.value),
        maxValue: Number(control.get('maxValue')?.value),
      })),
      countWeights: this.countWeightsArray.controls.map((control) => ({
        count: Number(control.get('count')?.value),
        weight: Number(control.get('weight')?.value),
      })),
    };
  }

  private reloadAfterSave(equipmentId: string): void {
    forkJoin({
      blueprints: this.adminBlueprintsService.getBlueprints(),
      equipmentCatalog: this.adminBlueprintsService.getEquipmentCatalog(),
      statCatalog: this.adminBlueprintsService.getStatCatalog(),
    }).subscribe({
      next: ({ blueprints, equipmentCatalog, statCatalog }) => {
        this.blueprints = blueprints;
        this.equipmentCatalog = equipmentCatalog;
        this.statCatalog = statCatalog;

        const savedBlueprint = this.blueprints.find(
          (item) => item.equipmentId === equipmentId
        );

        if (savedBlueprint) {
          this.openBlueprint(savedBlueprint.id);
          return;
        }

        this.startCreate();
      },
      error: (error: Error) => {
        this.submitError = error.message;
      },
    });
  }

  private clearFormArray(formArray: FormArray): void {
    while (formArray.length > 0) {
      formArray.removeAt(0);
    }
  }

  private isCriticalStatKey(statKey: string | null | undefined): boolean {
    if (!statKey) {
      return false;
    }

    const statName = this.statCatalog.find((item) => item.key === statKey)?.name;
    return statName ? isCriticalPercentStat(statName) : false;
  }
}
