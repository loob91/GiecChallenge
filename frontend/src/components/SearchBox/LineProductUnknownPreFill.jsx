import React from 'react';
import { useEffect, useState } from 'react';
import { useDispatch } from 'react-redux'
import { useTranslation } from 'react-i18next';
import { ProductSearchBox } from './ProductSearchBox';

export const LineProductUnknownPreFill = ({ unknowProduct, nameSelect, onChange }) => {
    const [selectedValue, setSelectedValue] = useState([]);
    const [quantity, setQuantity] = useState(unknowProduct.quantity);
    const [price, setPrice] = useState(unknowProduct.price);
    const dispatch = useDispatch()
    const { t } = useTranslation(["LineProduct"]);

    useEffect(() => {  
      onChange({
        id: nameSelect,
        quantity,
        price,
        selectedValue,
        translation: unknowProduct.product,
        currencyIsoCode: unknowProduct.currencyIsoCode
      });
    }, [dispatch, onChange, quantity, selectedValue, price, unknowProduct.currencyIsoCode, unknowProduct.product, nameSelect]);
    
    return (
      <>
        <fieldset key={`${nameSelect}fieldset`} className='mtop-10'>
          <legend>{t("product")}</legend>
          <div>
              <input type="text" name={`${nameSelect}quantity`} className="width-40 inlineflex form-control" id={`${nameSelect}quantity`} value={quantity} placeholder={t("quantity")} onChange={(e) => { setQuantity(e.target.value); }} />
              <input type="text" name={`${nameSelect}price`} className="width-40 inlineflex form-control" id={`${nameSelect}price`} value={price} placeholder={t("price")} onChange={(e) => { setPrice(e.target.value); }} />
          </div>
          <input type="text" key={`${nameSelect}nameAlreadyPreFill`} name={`${nameSelect}nameAlreadyPreFill`} className="width-100 inlineflex form-control" id={`${nameSelect}nameAlreadyPreFill`} onChange={() => {}} value={unknowProduct.product} />
          <input type="hidden" key={`${nameSelect}currency`} name={`${nameSelect}currency`} className="width-100 inlineflex form-control" id={`${nameSelect}currency`} onChange={() => {}} value={unknowProduct.currencyIsoCode} />
          <ProductSearchBox key={`${nameSelect}searchbox`} className='mbottom-10' nameSelect={nameSelect} onChange={(e) => setSelectedValue(e)}/>
        </fieldset>
      </>
    );
  };