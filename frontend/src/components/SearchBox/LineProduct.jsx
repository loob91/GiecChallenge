import React from 'react';
import { useEffect, useState } from 'react';
import { useDispatch } from 'react-redux'
import { useTranslation } from 'react-i18next';
import { ProductSearchBox } from './ProductSearchBox';

export const LineProduct = ({ nameSelect, onChange }) => {
    const [selectedValue, setSelectedValue] = useState([]);
    const [quantity, setQuantity] = useState('');
    const [price, setPrice] = useState('');
    const dispatch = useDispatch()
    const { t } = useTranslation(["LineProduct"]);

    useEffect(() => {  
      onChange({
        nameSelect,
        quantity,
        price,
        selectedValue
      });
    }, [dispatch, onChange, quantity, selectedValue, price, nameSelect]);
    
    return (
      <>
        <fieldset className='mtop-10'>
          <legend>{t("product")}</legend>
          <div>
              <input type="text" name={`${nameSelect}quantity`} className="width-40 inlineflex form-control" id={`${nameSelect}quantity`} value={quantity} placeholder={t("quantity")} onChange={(e) => { setQuantity(e.target.value); }} />
              <input type="text" name={`${nameSelect}price`} className="width-40 inlineflex form-control" id={`${nameSelect}price`} value={price} placeholder={t("price")} onChange={(e) => { setPrice(e.target.value); }} />
          </div>
          <ProductSearchBox key={nameSelect} className='mbottom-10' nameSelect={nameSelect} onChange={(e) => setSelectedValue(e)}/>
        </fieldset>
      </>
    );
  };